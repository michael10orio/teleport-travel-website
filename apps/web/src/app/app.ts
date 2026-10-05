import { Component, computed, signal } from '@angular/core';
import { RouterModule } from '@angular/router';

type DestinationCategory = 'All' | 'Coast' | 'Culture' | 'Nature';

interface Destination {
  name: string;
  region: string;
  category: Exclude<DestinationCategory, 'All'>;
  description: string;
  image: string;
  imageAlt: string;
}

@Component({
  imports: [RouterModule],
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  readonly categories: readonly DestinationCategory[] = [
    'All',
    'Coast',
    'Culture',
    'Nature',
  ];

  readonly destinations: readonly Destination[] = [
    {
      name: 'Lisbon',
      region: 'Portugal',
      category: 'Coast',
      description: 'Tile-bright streets meet the Atlantic breeze.',
      image:
        'https://images.unsplash.com/photo-1555881400-74d7acaacd8b?auto=format&fit=crop&w=1200&q=85',
      imageAlt: 'Colorful buildings on a Lisbon street',
    },
    {
      name: 'Kyoto',
      region: 'Japan',
      category: 'Culture',
      description: 'Garden paths, quiet temples, and old-town lanes.',
      image:
        'https://images.unsplash.com/photo-1493976040374-85c8e12f0c0e?auto=format&fit=crop&w=1200&q=85',
      imageAlt: 'Traditional wooden street in Kyoto',
    },
    {
      name: 'Banff',
      region: 'Canada',
      category: 'Nature',
      description: 'Alpine trails and glacial-blue lake water.',
      image:
        'https://images.unsplash.com/photo-1501785888041-af3ef285b470?auto=format&fit=crop&w=1200&q=85',
      imageAlt: 'Mountain peaks reflected in a turquoise lake',
    },
  ];

  readonly searchTerm = signal('');
  readonly selectedCategory = signal<DestinationCategory>('All');

  readonly filteredDestinations = computed(() => {
    const term = this.searchTerm().trim().toLocaleLowerCase();
    const category = this.selectedCategory();

    return this.destinations.filter((destination) => {
      const matchesCategory =
        category === 'All' || destination.category === category;
      const searchableText =
        `${destination.name} ${destination.region} ${destination.description}`.toLocaleLowerCase();

      return matchesCategory && searchableText.includes(term);
    });
  });

  updateSearch(event: Event): void {
    const input = event.target;

    if (input instanceof HTMLInputElement) {
      this.searchTerm.set(input.value);
    }
  }

  setCategory(category: DestinationCategory): void {
    this.selectedCategory.set(category);
  }
}
