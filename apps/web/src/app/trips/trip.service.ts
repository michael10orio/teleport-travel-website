import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Trip {
  id: number;
  name: string;
  startDate: string | null;
  endDate: string | null;
  createdAt: string;
}

export interface TripInput {
  name: string;
  startDate: string | null;
  endDate: string | null;
}

@Injectable({ providedIn: 'root' })
export class TripService {
  private readonly http = inject(HttpClient);
  private readonly tripsUrl = `${environment.apiBaseUrl}/trips`;

  getTrips(): Observable<Trip[]> {
    return this.http.get<Trip[]>(this.tripsUrl);
  }

  getTrip(id: number): Observable<Trip> {
    return this.http.get<Trip>(`${this.tripsUrl}/${id}`);
  }

  createTrip(request: TripInput): Observable<Trip> {
    return this.http.post<Trip>(this.tripsUrl, request);
  }

  updateTrip(id: number, request: TripInput): Observable<Trip> {
    return this.http.put<Trip>(`${this.tripsUrl}/${id}`, request);
  }

  deleteTrip(id: number): Observable<void> {
    return this.http.delete<void>(`${this.tripsUrl}/${id}`);
  }
}