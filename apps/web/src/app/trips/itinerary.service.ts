import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ItineraryItem {
  id: number;
  tripId: number;
  destinationId: number | null;
  activityId: number | null;
  sortOrder: number;
}

export interface ItineraryItemInput {
  destinationId: number | null;
  activityId: number | null;
  sortOrder: number;
}

@Injectable({ providedIn: 'root' })
export class ItineraryService {
  private readonly http = inject(HttpClient);

  getItems(tripId: number): Observable<ItineraryItem[]> {
    return this.http.get<ItineraryItem[]>(this.itemsUrl(tripId));
  }

  addItem(tripId: number, request: ItineraryItemInput): Observable<ItineraryItem> {
    return this.http.post<ItineraryItem>(this.itemsUrl(tripId), request);
  }

  updateSortOrder(
    tripId: number,
    itemId: number,
    sortOrder: number,
  ): Observable<ItineraryItem> {
    return this.http.put<ItineraryItem>(`${this.itemsUrl(tripId)}/${itemId}`, {
      sortOrder,
    });
  }

  deleteItem(tripId: number, itemId: number): Observable<void> {
    return this.http.delete<void>(`${this.itemsUrl(tripId)}/${itemId}`);
  }

  private itemsUrl(tripId: number): string {
    return `${environment.apiBaseUrl}/trips/${tripId}/itinerary-items`;
  }
}