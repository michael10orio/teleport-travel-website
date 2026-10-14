import { CurrencyPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { Activity, Destination, DestinationService } from '../destination.service';
import { Expense, ExpenseInput, ExpenseService } from './expense.service';
import { ItineraryItem, ItineraryItemInput, ItineraryService } from './itinerary.service';
import { Trip, TripService } from './trip.service';

@Component({
  selector: 'app-trip-detail-page',
  standalone: true,
  imports: [CurrencyPipe, ReactiveFormsModule, RouterLink],
  templateUrl: './trip-detail.page.html',
  styleUrl: './trip-detail.page.css',
})
export class TripDetailPage {
  private loadGeneration = 0;
  private readonly route = inject(ActivatedRoute);
  private readonly formBuilder = inject(FormBuilder);
  private readonly tripService = inject(TripService);
  private readonly itineraryService = inject(ItineraryService);
  private readonly expenseService = inject(ExpenseService);
  private readonly destinationService = inject(DestinationService);

  readonly trip = signal<Trip | null>(null);
  readonly itinerary = signal<ItineraryItem[]>([]);
  readonly expenses = signal<Expense[]>([]);
  readonly destinations = signal<Destination[]>([]);
  readonly activityNames = signal<Record<number, string>>({});
  readonly isLoading = signal(true);
  readonly error = signal<string | null>(null);
  readonly itineraryLoading = signal(false);
  readonly itineraryError = signal<string | null>(null);
  readonly destinationsError = signal<string | null>(null);
  readonly activityOptionsLoading = signal(false);
  readonly activityOptionsError = signal<string | null>(null);
  readonly activityNamesLoading = signal(false);
  readonly activityNamesError = signal<string | null>(null);
  readonly expensesLoading = signal(false);
  readonly expensesError = signal<string | null>(null);
  readonly itineraryOperationError = signal<string | null>(null);
  readonly expenseOperationError = signal<string | null>(null);
  readonly isSavingItinerary = signal(false);
  readonly isSavingExpense = signal(false);
  readonly editingExpenseId = signal<number | null>(null);

  readonly selectedDestinationId = signal<number | null>(null);
  readonly availableActivities = signal<Activity[]>([]);
  private readonly currentTripId = signal<number | null>(null);
  readonly totalExpenses = computed(() =>
    this.expenses().reduce((total, expense) => total + expense.amount, 0),
  );

  readonly itineraryForm = this.formBuilder.nonNullable.group({
    destinationId: ['', Validators.required],
    activityId: [''],
  });

  readonly expenseForm = this.formBuilder.nonNullable.group({
    description: ['', [Validators.required, Validators.maxLength(200)]],
    amount: [0, [Validators.required, Validators.min(0.01)]],
    expenseDate: [this.today()],
  });

  constructor() {
    this.route.paramMap.pipe(takeUntilDestroyed()).subscribe((params) => {
      const tripId = Number(params.get('id'));
      if (!Number.isSafeInteger(tripId) || tripId < 1) {
        this.currentTripId.set(null);
        this.trip.set(null);
        this.isLoading.set(false);
        this.error.set('This trip link is not valid.');
        return;
      }
      this.currentTripId.set(tripId);
      this.loadTrip(tripId);
    });
  }

  destinationChanged(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    const destinationId = value ? Number(value) : null;
    this.selectedDestinationId.set(destinationId);
    this.itineraryForm.controls.activityId.setValue('');
    this.availableActivities.set([]);
    this.activityOptionsError.set(null);

    if (destinationId === null) {
      this.activityOptionsLoading.set(false);
      return;
    }

    this.loadActivities(destinationId);
  }

  retryActivityOptions(): void {
    const destinationId = this.selectedDestinationId();
    if (destinationId !== null) {
      this.loadActivities(destinationId);
    }
  }

  private loadActivities(destinationId: number): void {
    this.activityOptionsLoading.set(true);
    this.destinationService.getActivities(destinationId).subscribe({
      next: (activities) => {
        if (this.selectedDestinationId() !== destinationId) {
          return;
        }
        this.availableActivities.set(activities);
        this.activityOptionsLoading.set(false);
      },
      error: () => {
        if (this.selectedDestinationId() !== destinationId) {
          return;
        }
        this.activityOptionsError.set('Activities could not be loaded.');
        this.activityOptionsLoading.set(false);
      },
    });
  }

  addItineraryItem(): void {
    this.itineraryOperationError.set(null);
    if (this.itineraryForm.invalid || this.isSavingItinerary()) {
      this.itineraryForm.markAllAsTouched();
      return;
    }

    const destinationId = Number(this.itineraryForm.controls.destinationId.value);
    const activityId = this.itineraryForm.controls.activityId.value;
    const request: ItineraryItemInput = activityId
      ? { destinationId: null, activityId: Number(activityId), sortOrder: this.nextSortOrder() }
      : { destinationId, activityId: null, sortOrder: this.nextSortOrder() };
    const tripId = this.tripId();

    this.isSavingItinerary.set(true);
    this.itineraryService.addItem(tripId, request).subscribe({
      next: (item) => {
        if (tripId !== this.tripId()) {
          return;
        }
        this.itinerary.update((items) => [...items, item].sort((a, b) => a.sortOrder - b.sortOrder));
        this.itineraryForm.reset({ destinationId: '', activityId: '' });
        this.selectedDestinationId.set(null);
        this.availableActivities.set([]);
        this.activityOptionsLoading.set(false);
        this.activityOptionsError.set(null);
        this.isSavingItinerary.set(false);
      },
      error: () => {
        if (tripId !== this.tripId()) {
          return;
        }
        this.itineraryOperationError.set('The itinerary item could not be added. Try again.');
        this.isSavingItinerary.set(false);
      },
    });
  }

  moveItineraryItem(item: ItineraryItem, direction: -1 | 1): void {
    this.itineraryOperationError.set(null);
    const orderedItems = [...this.itinerary()].sort((a, b) => a.sortOrder - b.sortOrder);
    const currentIndex = orderedItems.findIndex((current) => current.id === item.id);
    const adjacentItem = orderedItems[currentIndex + direction];
    if (!adjacentItem || this.isSavingItinerary()) {
      return;
    }

    const tripId = this.tripId();
    this.isSavingItinerary.set(true);
    forkJoin([
      this.itineraryService.updateSortOrder(tripId, item.id, adjacentItem.sortOrder),
      this.itineraryService.updateSortOrder(tripId, adjacentItem.id, item.sortOrder),
    ]).subscribe({
      next: ([updatedItem, updatedAdjacent]) => {
        if (tripId !== this.tripId()) {
          return;
        }
        const updatedById = new Map([
          [updatedItem.id, updatedItem],
          [updatedAdjacent.id, updatedAdjacent],
        ]);
        this.itinerary.update((items) =>
          items.map((current) => updatedById.get(current.id) ?? current)
            .sort((a, b) => a.sortOrder - b.sortOrder),
        );
        this.isSavingItinerary.set(false);
      },
      error: () => {
        if (tripId !== this.tripId()) {
          return;
        }
        this.itineraryOperationError.set('The itinerary order could not be changed. Reload the page and try again.');
        this.isSavingItinerary.set(false);
        this.loadItinerary();
      },
    });
  }

  deleteItineraryItem(item: ItineraryItem): void {
    this.itineraryOperationError.set(null);
    if (this.isSavingItinerary() || !globalThis.confirm(`Remove "${this.itineraryItemName(item)}" from the itinerary?`)) {
      return;
    }

    const tripId = this.tripId();
    this.isSavingItinerary.set(true);
    this.itineraryService.deleteItem(tripId, item.id).subscribe({
      next: () => {
        if (tripId !== this.tripId()) {
          return;
        }
        this.itinerary.update((items) => items.filter((current) => current.id !== item.id));
        this.isSavingItinerary.set(false);
      },
      error: () => {
        if (tripId !== this.tripId()) {
          return;
        }
        this.itineraryOperationError.set('The itinerary item could not be removed. Try again.');
        this.isSavingItinerary.set(false);
      },
    });
  }

  itineraryItemName(item: ItineraryItem): string {
    if (item.destinationId !== null) {
      return this.destinations().find((destination) => destination.id === item.destinationId)?.name
        ?? `Destination #${item.destinationId}`;
    }
    if (item.activityId !== null) {
      return this.activityNames()[item.activityId] ?? `Activity #${item.activityId}`;
    }
    return 'Unknown itinerary item';
  }

  saveExpense(): void {
    this.expenseOperationError.set(null);
    if (this.expenseForm.invalid || this.isSavingExpense()) {
      this.expenseForm.markAllAsTouched();
      return;
    }

    const values = this.expenseForm.getRawValue();
    const request: ExpenseInput = {
      description: values.description.trim(),
      amount: values.amount,
      expenseDate: values.expenseDate || null,
    };
    const editingId = this.editingExpenseId();
    const tripId = this.tripId();
    const operation = editingId === null
      ? this.expenseService.addExpense(tripId, request)
      : this.expenseService.updateExpense(tripId, editingId, request);

    this.isSavingExpense.set(true);
    operation.subscribe({
      next: (expense) => {
        if (tripId !== this.tripId()) {
          return;
        }
        this.expenses.update((items) => (editingId === null
          ? [expense, ...items]
          : items.map((current) => current.id === expense.id ? expense : current))
          .sort((a, b) => b.expenseDate.localeCompare(a.expenseDate)));
        this.resetExpenseForm();
        this.isSavingExpense.set(false);
      },
      error: () => {
        if (tripId !== this.tripId()) {
          return;
        }
        this.expenseOperationError.set('The expense could not be saved. Check the details and try again.');
        this.isSavingExpense.set(false);
      },
    });
  }

  editExpense(expense: Expense): void {
    if (this.isSavingExpense()) {
      return;
    }
    this.editingExpenseId.set(expense.id);
    this.expenseForm.setValue({
      description: expense.description,
      amount: expense.amount,
      expenseDate: expense.expenseDate,
    });
    this.expenseOperationError.set(null);
  }

  deleteExpense(expense: Expense): void {
    this.expenseOperationError.set(null);
    if (this.isSavingExpense() || !globalThis.confirm(`Delete the expense "${expense.description}"?`)) {
      return;
    }

    const tripId = this.tripId();
    this.isSavingExpense.set(true);
    this.expenseService.deleteExpense(tripId, expense.id).subscribe({
      next: () => {
        if (tripId !== this.tripId()) {
          return;
        }
        this.expenses.update((items) => items.filter((current) => current.id !== expense.id));
        if (this.editingExpenseId() === expense.id) {
          this.resetExpenseForm();
        }
        this.isSavingExpense.set(false);
      },
      error: () => {
        if (tripId !== this.tripId()) {
          return;
        }
        this.expenseOperationError.set('The expense could not be deleted. Try again.');
        this.isSavingExpense.set(false);
      },
    });
  }

  resetExpenseForm(): void {
    this.editingExpenseId.set(null);
    this.expenseOperationError.set(null);
    this.expenseForm.reset({
      description: '',
      amount: 0,
      expenseDate: this.today(),
    });
  }

  retryItinerary(): void {
    this.loadItinerary();
  }

  retryExpenses(): void {
    this.loadExpenses();
  }

  retryDestinations(): void {
    this.loadDestinations();
  }

  private loadTrip(tripId: number): void {
    const generation = ++this.loadGeneration;
    this.trip.set(null);
    this.itinerary.set([]);
    this.expenses.set([]);
    this.destinations.set([]);
    this.activityNames.set({});
    this.itineraryError.set(null);
    this.expensesError.set(null);
    this.destinationsError.set(null);
    this.activityNamesError.set(null);
    this.activityNamesLoading.set(false);
    this.activityOptionsLoading.set(false);
    this.activityOptionsError.set(null);
    this.itineraryLoading.set(false);
    this.expensesLoading.set(false);
    this.itineraryOperationError.set(null);
    this.expenseOperationError.set(null);
    this.isSavingItinerary.set(false);
    this.isSavingExpense.set(false);
    this.editingExpenseId.set(null);
    this.selectedDestinationId.set(null);
    this.availableActivities.set([]);
    this.itineraryForm.reset({ destinationId: '', activityId: '' });
    this.resetExpenseForm();
    this.isLoading.set(true);
    this.error.set(null);

    this.tripService.getTrip(tripId).subscribe({
      next: (trip) => {
        if (generation !== this.loadGeneration) {
          return;
        }
        this.trip.set(trip);
        this.isLoading.set(false);
        this.loadItinerary(tripId, generation);
        this.loadExpenses(tripId, generation);
        this.loadDestinations(generation);
      },
      error: () => {
        if (generation !== this.loadGeneration) {
          return;
        }
        this.isLoading.set(false);
        this.error.set('Trip could not be loaded. Check that the API and database are available.');
      },
    });
  }

  private loadItinerary(tripId = this.tripId(), generation = this.loadGeneration): void {
    this.itineraryLoading.set(true);
    this.itineraryError.set(null);
    this.itineraryService.getItems(tripId).subscribe({
      next: (items) => {
        if (generation !== this.loadGeneration) {
          return;
        }
        this.itinerary.set(items);
        this.itineraryLoading.set(false);
      },
      error: () => {
        if (generation !== this.loadGeneration) {
          return;
        }
        this.itineraryError.set('The itinerary could not be loaded.');
        this.itineraryLoading.set(false);
      },
    });
  }

  private loadExpenses(tripId = this.tripId(), generation = this.loadGeneration): void {
    this.expensesLoading.set(true);
    this.expensesError.set(null);
    this.expenseService.getExpenses(tripId).subscribe({
      next: (items) => {
        if (generation !== this.loadGeneration) {
          return;
        }
        this.expenses.set(items);
        this.expensesLoading.set(false);
      },
      error: () => {
        if (generation !== this.loadGeneration) {
          return;
        }
        this.expensesError.set('Expenses could not be loaded.');
        this.expensesLoading.set(false);
      },
    });
  }

  private loadDestinations(generation = this.loadGeneration): void {
    this.destinationsError.set(null);
    this.activityNamesError.set(null);
    this.destinationService.getDestinations().subscribe({
      next: (destinations) => {
        if (generation !== this.loadGeneration) {
          return;
        }
        this.destinations.set(destinations);
        this.loadActivityNames(destinations, generation);
      },
      error: () => {
        if (generation === this.loadGeneration) {
          this.destinationsError.set('Destinations could not be loaded. Itinerary names may be unavailable.');
        }
      },
    });
  }

  private loadActivityNames(destinations: Destination[], generation: number): void {
    if (destinations.length === 0) {
      this.activityNames.set({});
      return;
    }

    this.activityNamesLoading.set(true);
    forkJoin(destinations.map((destination) =>
      this.destinationService.getActivities(destination.id),
    )).subscribe({
      next: (activityGroups) => {
        if (generation !== this.loadGeneration) {
          return;
        }
        this.activityNames.set(Object.fromEntries(
          activityGroups.flat().map((activity) => [activity.id, activity.name]),
        ));
        this.activityNamesLoading.set(false);
      },
      error: () => {
        if (generation !== this.loadGeneration) {
          return;
        }
        this.activityNamesError.set('Activity names could not be loaded.');
        this.activityNamesLoading.set(false);
      },
    });
  }

  private tripId(): number {
    return this.currentTripId() ?? Number(this.route.snapshot.paramMap.get('id'));
  }

  private nextSortOrder(): number {
    return this.itinerary().reduce((maximum, item) => Math.max(maximum, item.sortOrder), -1) + 1;
  }

  private today(): string {
    const date = new Date();
    const localDate = new Date(date.getTime() - date.getTimezoneOffset() * 60_000);
    return localDate.toISOString().slice(0, 10);
  }
}
