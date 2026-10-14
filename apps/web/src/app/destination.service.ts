import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

export interface Destination {
  id: number;
  name: string;
  region: string;
  category: string;
  description: string;
  imageUrl: string | null;
}

export interface Activity {
  id: number;
  destinationId: number;
  name: string;
  description: string;
}

@Injectable({ providedIn: 'root' })
export class DestinationService {
  private readonly http = inject(HttpClient);

  getDestinations(): Observable<Destination[]> {
    return this.http.get<Destination[]>(`${environment.apiBaseUrl}/destinations`);
  }

  getActivities(destinationId: number): Observable<Activity[]> {
    return this.http.get<Activity[]>(
      `${environment.apiBaseUrl}/destinations/${destinationId}/activities`,
    );
  }
}