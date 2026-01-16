import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// Define the shape of our Ticket data
export interface Ticket {
  id: number;
  problemTitle: string;
  problemDescription: string;
  status: string;
  priority: string;
  category: string;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5299/api/Tickets'; // Your .NET API URL

  getTickets(): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(this.apiUrl);
  }
}