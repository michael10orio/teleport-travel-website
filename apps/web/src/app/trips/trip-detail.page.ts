import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Activity, Destination, DestinationService } from '../destination.service';
import { Expense, ExpenseService } from './expense.service';
import { ItineraryItem, ItineraryItemInput, ItineraryService } from './itinerary.service';
import { Trip, TripService } from './trip.service';
import { forkJoin } from 'rxjs';

type ItineraryItemType = 'destination' | 'activity';

@Component({
  selector: 'app-trip-detail-page',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './trip-detail.page.html',
  styleUrl: './trip-detail.page.css',
})
export class TripDetailPage {
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly tripService = inject(TripService);
  private readonly destinationService = inject(DestinationService);
  private readonly itineraryService = inject(ItineraryService);
  private readonly expenseService = inject(ExpenseService);

  readonly trip = signal<Trip | null>(null);
  readonly itinerary = signal<ItineraryItem[]>([]);
  readonly itineraryActivities = signal<Activity[]>([]);
  readonly destinations = signal<Destination[]>([]);
  readonly availableActivities = signal<Activity[]>([]);
  readonly expenses = signal<Expense[]>([]);
  readonly isLoading = signal(true);
  readonly error = signal<string | null>(null);
  readonly isLoadingItinerary = signal(true);
  readonly itineraryLoadFailed = signal(false);
  readonly isLoadingDestinations = signal(false);
  readonly isLoadingActivities = signal(false);
  readonly itineraryOptionsFailed = signal(false);
  readonly itineraryOperationFailed = signal(false);
  readonly isAddingItineraryItem = signal(false);
  readonly itineraryItemType = signal<ItineraryItemType>('destination');
  readonly selectedDestinationId = signal<number | null>(null);
  readonly selectedActivityId = signal<number | null>(null);
  readonly isLoadingExpenses = signal(true);
  readonly expenseLoadFailed = signal(false);
  readonly expenseOperationFailed = signal(false);
  readonly isSavingExpense = signal(false);
  readonly editingExpenseId = signal<number | null>(null);

  readonly expenseForm = this.formBuilder.nonNullable.group({
    description: ['', [Validators.required, Validators.maxLength(200)]],
    amount: [0, [Validators.required, Validators.min(0.01)]],
    expenseDate: [''],
  });

  readonly tripSummary = computed(() => {
    const currentTrip = this.trip();
    if (!currentTrip) {
      return 'Trip details';
    }

    const start = currentTrip.startDate ?? 'No start date';
    const end = currentTrip.endDate ?? 'No end date';
    return `${currentTrip.name} · ${start} → ${end}`;
  });

  readonly tripId = Number(this.route.snapshot.paramMap.get('id'));

  itineraryItemLabel(item: ItineraryItem): string {
    if (item.destinationId !== null) {
      const destination = this.destinations().find((entry) => entry.id === item.destinationId);
      return destination ? `Destination: ${destination.name}` : `Destination #${item.destinationId}`;
    }

    if (item.activityId !== null) {
      const activity = this.itineraryActivities().find((entry) => entry.id === item.activityId);
      return activity ? `Activity: ${activity.name}` : `Activity #${item.activityId}`;
    }

    return 'Unknown itinerary item';
  }

  constructor() {
    this.loadTrip();
  }

  private loadTrip(): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.tripService.getTrip(this.tripId).subscribe({
      next: (trip) => {
        this.trip.set(trip);
        this.isLoading.set(false);
        this.loadRelatedData(trip.id);
      },
      error: () => {
        this.isLoading.set(false);
        this.error.set('Trip could not be loaded.');
      },
    });
  }

  private loadRelatedData(tripId: number): void {
    this.loadDestinations();

    this.isLoadingItinerary.set(true);
    this.itineraryLoadFailed.set(false);
    this.itineraryService.getItems(tripId).subscribe({
      next: (items) => {
        this.itinerary.set(items);
        this.isLoadingItinerary.set(false);
      },
      error: () => {
        this.itineraryLoadFailed.set(true);
        this.isLoadingItinerary.set(false);
      },
    });

    this.isLoadingExpenses.set(true);
    this.expenseLoadFailed.set(false);
    this.expenseService.getExpenses(tripId).subscribe({
      next: (items) => {
        this.expenses.set(items);
        this.isLoadingExpenses.set(false);
      },
      error: () => {
        this.expenseLoadFailed.set(true);
        this.isLoadingExpenses.set(false);
      },
    });
  }

  setItineraryItemType(type: ItineraryItemType): void {
    this.itineraryItemType.set(type);
    this.selectedActivityId.set(null);
    this.availableActivities.set([]);

    if (type === 'activity' && this.selectedDestinationId() !== null) {
      this.loadActivities(this.selectedDestinationId()!);
    }
  }

  selectDestination(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    const destinationId = value ? Number(value) : null;
    this.selectedDestinationId.set(destinationId);
    this.selectedActivityId.set(null);
    this.availableActivities.set([]);

    if (destinationId !== null && this.itineraryItemType() === 'activity') {
      this.loadActivities(destinationId);
    }
  }

  selectActivity(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.selectedActivityId.set(value ? Number(value) : null);
  }

  addItineraryItem(): void {
    const destinationId = this.itineraryItemType() === 'destination'
      ? this.selectedDestinationId()
      : null;
    const activityId = this.itineraryItemType() === 'activity'
      ? this.selectedActivityId()
      : null;

    if ((destinationId === null) === (activityId === null)) {
      return;
    }

    const sortOrder = this.itinerary().reduce(
      (maximum, item) => Math.max(maximum, item.sortOrder),
      -1,
    ) + 1;
    const request: ItineraryItemInput = { destinationId, activityId, sortOrder };

    this.itineraryOperationFailed.set(false);
    this.isAddingItineraryItem.set(true);
    this.itineraryService.addItem(this.tripId, request).subscribe({
      next: (item) => {
        this.itinerary.update((current) => [...current, item].sort((a, b) => a.sortOrder - b.sortOrder));
        this.selectedActivityId.set(null);
        this.isAddingItineraryItem.set(false);
      },
      error: () => {
        this.itineraryOperationFailed.set(true);
        this.isAddingItineraryItem.set(false);
      },
    });
  }

  removeItineraryItem(item: ItineraryItem): void {
    this.itineraryOperationFailed.set(false);
    this.itineraryService.deleteItem(this.tripId, item.id).subscribe({
      next: () => this.itinerary.update((current) => current.filter((entry) => entry.id !== item.id)),
      error: () => this.itineraryOperationFailed.set(true),
    });
  }

  saveExpense(): void {
    this.expenseOperationFailed.set(false);
    if (this.expenseForm.invalid || this.trip() === null) {
      this.expenseForm.markAllAsTouched();
      return;
    }

    const values = this.expenseForm.getRawValue();
    const request = {
      description: values.description.trim(),
      amount: values.amount,
      expenseDate: values.expenseDate || null,
    };
    const editingId = this.editingExpenseId();
    const operation = editingId === null
      ? this.expenseService.addExpense(this.tripId, request)
      : this.expenseService.updateExpense(this.tripId, editingId, request);

    this.isSavingExpense.set(true);
    operation.subscribe({
      next: (expense) => {
        this.expenses.update((current) => editingId === null
          ? [expense, ...current]
          : current.map((item) => item.id === expense.id ? expense : item));
        this.resetExpenseForm();
        this.isSavingExpense.set(false);
      },
      error: () => {
        this.expenseOperationFailed.set(true);
        this.isSavingExpense.set(false);
      },
    });
  }

  editExpense(expense: Expense): void {
    this.editingExpenseId.set(expense.id);
    this.expenseForm.setValue({
      description: expense.description,
      amount: expense.amount,
      expenseDate: expense.expenseDate,
    });
    this.expenseOperationFailed.set(false);
  }

  deleteExpense(expense: Expense): void {
    if (!globalThis.confirm(`Delete the expense "${expense.description}"?`)) {
      return;
    }

    this.expenseService.deleteExpense(this.tripId, expense.id).subscribe({
      next: () => this.expenses.update((current) => current.filter((item) => item.id !== expense.id)),
      error: () => this.expenseOperationFailed.set(true),
    });
  }

  resetExpenseForm(): void {
    this.editingExpenseId.set(null);
    this.expenseForm.reset({ description: '', amount: 0, expenseDate: '' });
  }

  private loadDestinations(): void {
    this.isLoadingDestinations.set(true);
    this.itineraryOptionsFailed.set(false);
    this.destinationService.getDestinations().subscribe({
      next: (destinations) => {
        this.destinations.set(destinations);
        this.isLoadingDestinations.set(false);
        if (destinations.length > 0) {
          forkJoin(destinations.map((destination) => this.destinationService.getActivities(destination.id)))
            .subscribe({
              next: (activityGroups) => this.itineraryActivities.set(activityGroups.flat()),
              error: () => this.itineraryActivities.set([]),
            });
        }
      },
      error: () => {
        this.itineraryOptionsFailed.set(true);
        this.isLoadingDestinations.set(false);
      },
    });
  }

  private loadActivities(destinationId: number): void {
    this.isLoadingActivities.set(true);
    this.itineraryOptionsFailed.set(false);
    this.destinationService.getActivities(destinationId).subscribe({
      next: (activities) => {
        this.availableActivities.set(activities);
        this.isLoadingActivities.set(false);
      },
      error: () => {
        this.itineraryOptionsFailed.set(true);
        this.isLoadingActivities.set(false);
      },
    });
  }
}
