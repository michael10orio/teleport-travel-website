import { Component, inject, OnInit, signal } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Trip, TripInput, TripService } from './trip.service';

const tripDateRangeValidator: ValidatorFn = (control) => {
  const startDate = control.get('startDate')?.value as string | undefined;
  const endDate = control.get('endDate')?.value as string | undefined;

  return startDate && endDate && endDate < startDate
    ? { dateRange: true }
    : null;
};

@Component({
  selector: 'app-trips-page',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './trips.page.html',
  styleUrl: './trips.page.css',
})
export class TripsPage implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly tripService = inject(TripService);

  readonly trips = signal<Trip[]>([]);
  readonly isLoading = signal(true);
  readonly loadFailed = signal(false);
  readonly operationFailed = signal(false);
  readonly isSaving = signal(false);
  readonly editingTripId = signal<number | null>(null);

  readonly tripForm = this.formBuilder.nonNullable.group(
    {
      name: ['', [Validators.required, Validators.maxLength(120)]],
      startDate: [''],
      endDate: [''],
    },
    { validators: tripDateRangeValidator },
  );

  ngOnInit(): void {
    this.loadTrips();
  }

  loadTrips(): void {
    this.isLoading.set(true);
    this.loadFailed.set(false);

    this.tripService.getTrips().subscribe({
      next: (trips) => {
        this.trips.set(trips);
        this.isLoading.set(false);
      },
      error: () => {
        this.loadFailed.set(true);
        this.isLoading.set(false);
      },
    });
  }

  saveTrip(): void {
    this.operationFailed.set(false);

    if (this.tripForm.invalid) {
      this.tripForm.markAllAsTouched();
      return;
    }

    const values = this.tripForm.getRawValue();
    const request: TripInput = {
      name: values.name.trim(),
      startDate: values.startDate || null,
      endDate: values.endDate || null,
    };
    const editingTripId = this.editingTripId();
    const operation = editingTripId === null
      ? this.tripService.createTrip(request)
      : this.tripService.updateTrip(editingTripId, request);

    this.isSaving.set(true);
    operation.subscribe({
      next: (trip) => {
        this.trips.update((currentTrips) => editingTripId === null
          ? [trip, ...currentTrips]
          : currentTrips.map((currentTrip) => currentTrip.id === trip.id ? trip : currentTrip));
        this.resetForm();
        this.isSaving.set(false);
      },
      error: () => {
        this.operationFailed.set(true);
        this.isSaving.set(false);
      },
    });
  }

  editTrip(trip: Trip): void {
    this.editingTripId.set(trip.id);
    this.tripForm.setValue({
      name: trip.name,
      startDate: trip.startDate ?? '',
      endDate: trip.endDate ?? '',
    });
    this.operationFailed.set(false);
  }

  deleteTrip(trip: Trip): void {
    if (!globalThis.confirm(`Delete the trip "${trip.name}" and its associated items?`)) {
      return;
    }

    this.tripService.deleteTrip(trip.id).subscribe({
      next: () => {
        this.trips.update((currentTrips) => currentTrips.filter((item) => item.id !== trip.id));
      },
      error: () => this.operationFailed.set(true),
    });
  }

  resetForm(): void {
    this.editingTripId.set(null);
    this.tripForm.reset({ name: '', startDate: '', endDate: '' });
  }
}