import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Activity, Destination, DestinationService } from '../../destination.service';

@Component({
  selector: 'app-destination-detail-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './destination-detail.page.html',
  styleUrl: './destination-detail.page.css',
})
export class DestinationDetailPage {
  private readonly route = inject(ActivatedRoute);
  private readonly destinationService = inject(DestinationService);

  readonly destination = signal<Destination | null>(null);
  readonly activities = signal<Activity[]>([]);
  readonly isLoading = signal(true);
  readonly isLoadingActivities = signal(false);
  readonly loadFailed = signal(false);
  readonly loadError = signal('');
  readonly activitiesFailed = signal(false);
  readonly invalidId = !this.isValidId(Number(this.route.snapshot.paramMap.get('id')));
  readonly destinationId = Number(this.route.snapshot.paramMap.get('id'));

  constructor() {
    this.loadDestination();
  }

  retryLoad(): void {
    this.loadDestination();
  }

  retryActivities(): void {
    this.loadActivities();
  }

  private loadDestination(): void {
    if (this.invalidId) {
      this.isLoading.set(false);
      this.loadFailed.set(true);
      return;
    }

    this.isLoading.set(true);
    this.loadFailed.set(false);
    this.loadError.set('');
    this.destination.set(null);
    this.activities.set([]);
    this.activitiesFailed.set(false);

    this.destinationService.getDestination(this.destinationId).subscribe({
      next: (destination) => {
        this.destination.set(destination);
        this.isLoading.set(false);
        this.loadActivities();
      },
      error: (error: unknown) => {
        this.loadFailed.set(true);
        this.isLoading.set(false);
        this.loadError.set(
          error instanceof HttpErrorResponse && error.status === 404
            ? 'This destination could not be found. It may have been removed.'
            : 'Destination details could not be loaded. Check your connection and try again.',
        );
      },
    });
  }

  private loadActivities(): void {
    this.isLoadingActivities.set(true);
    this.activitiesFailed.set(false);

    this.destinationService.getActivities(this.destinationId).subscribe({
      next: (activities) => {
        this.activities.set(activities);
        this.isLoadingActivities.set(false);
      },
      error: () => {
        this.activitiesFailed.set(true);
        this.isLoadingActivities.set(false);
      },
    });
  }

  private isValidId(id: number): boolean {
    return Number.isSafeInteger(id) && id > 0;
  }
}
