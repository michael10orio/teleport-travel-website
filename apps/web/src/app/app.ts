import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Destination, DestinationService } from './destination.service';

type DestinationCategory = 'All' | string;

@Component({
  imports: [RouterModule],
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  private readonly destinationService = inject(DestinationService);
  readonly destinations = signal<Destination[]>([]);
  readonly isLoading = signal(true);
  readonly loadFailed = signal(false);
  readonly searchTerm = signal('');
  readonly selectedCategory = signal<string>('All');

  readonly categories = computed(() => [
    'All',
    ...new Set(this.destinations().map((destination) => destination.category)),
  ]);

  readonly filteredDestinations = computed(() => {
    const term = this.searchTerm().trim().toLocaleLowerCase();
    const category = this.selectedCategory();

    return this.destinations().filter((destination) => {
      const matchesCategory =
        category === 'All' || destination.category === category;
      const searchableText =
        `${destination.name} ${destination.region} ${destination.description}`.toLocaleLowerCase();

      return matchesCategory && searchableText.includes(term);
    });
  });

  ngOnInit(): void {
    this.loadDestinations();
  }

  retryLoad(): void {
    this.loadDestinations();
  }

  updateSearch(event: Event): void {
    const input = event.target;

    if (input instanceof HTMLInputElement) {
      this.searchTerm.set(input.value);
    }
  }

  setCategory(category: string): void {
    this.selectedCategory.set(category);
  }

  private loadDestinations(): void {
    this.isLoading.set(true);
    this.loadFailed.set(false);

    this.destinationService.getDestinations().subscribe({
      next: (destinations) => {
        this.destinations.set(destinations);
        this.isLoading.set(false);
      },
      error: () => {
        this.loadFailed.set(true);
        this.isLoading.set(false);
      },
    });
  }
}
