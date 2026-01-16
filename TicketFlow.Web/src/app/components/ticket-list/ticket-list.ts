import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common'; // Important for displaying dates/loops
import { TicketService, Ticket } from '../../services/ticket';

@Component({
  selector: 'app-ticket-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ticket-list.html',
  styleUrl: './ticket-list.scss'
})
export class TicketList implements OnInit {
  private ticketService = inject(TicketService);
  
  // We use a 'signal' to hold data (Modern Angular best practice)
  tickets = signal<Ticket[]>([]);

  ngOnInit() {
    this.loadTickets();
    
    // Auto-refresh every 5 seconds to see AI updates live!
    setInterval(() => this.loadTickets(), 5000);
  }

  loadTickets() {
    this.ticketService.getTickets().subscribe(data => {
      this.tickets.set(data);
    });
  }
}