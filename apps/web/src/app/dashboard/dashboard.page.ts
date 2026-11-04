import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Trip, TripService } from '../trips/trip.service';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './dashboard.page.html',
})
export class DashboardPage implements OnInit {
  private readonly tripService = inject(TripService);

  readonly trips = signal<Trip[]>([]);
  readonly isLoading = signal(true);
  readonly loadFailed = signal(false);
  readonly tripCount = computed(() => this.trips().length);

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
}
