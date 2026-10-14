import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Expense {
  id: number;
  tripId: number;
  description: string;
  amount: number;
  expenseDate: string;
}

export interface ExpenseInput {
  description: string;
  amount: number;
  expenseDate: string | null;
}

@Injectable({ providedIn: 'root' })
export class ExpenseService {
  private readonly http = inject(HttpClient);

  getExpenses(tripId: number): Observable<Expense[]> {
    return this.http.get<Expense[]>(this.expensesUrl(tripId));
  }

  addExpense(tripId: number, request: ExpenseInput): Observable<Expense> {
    return this.http.post<Expense>(this.expensesUrl(tripId), request);
  }

  updateExpense(
    tripId: number,
    expenseId: number,
    request: ExpenseInput,
  ): Observable<Expense> {
    return this.http.put<Expense>(`${this.expensesUrl(tripId)}/${expenseId}`, request);
  }

  deleteExpense(tripId: number, expenseId: number): Observable<void> {
    return this.http.delete<void>(`${this.expensesUrl(tripId)}/${expenseId}`);
  }

  private expensesUrl(tripId: number): string {
    return `${environment.apiBaseUrl}/trips/${tripId}/expenses`;
  }
}