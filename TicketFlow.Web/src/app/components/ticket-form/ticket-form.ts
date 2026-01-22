import { Component, output, inject } from '@angular/core'; // Note 'output' is new in Angular 17+
import { FormsModule } from '@angular/forms';
import { TicketService } from '../../services/ticket';

@Component({
  selector: 'app-ticket-form',
  standalone: true,
  imports: [FormsModule], // <--- Needed for [(ngModel)]
  templateUrl: './ticket-form.html',
  styleUrl: './ticket-form.scss'
})
export class TicketForm {
  private ticketService = inject(TicketService);

  // This event tells the parent (TicketList) to refresh when we are done
  ticketCreated = output<void>();

  title = '';
  description = '';
  isSubmitting = false;

  onSubmit() {
    if (!this.title || !this.description) return;

    this.isSubmitting = true;

    const newTicket = { title: this.title, description: this.description };

    this.ticketService.createTicket(newTicket).subscribe(() => {
      this.title = '';
      this.description = '';
      this.isSubmitting = false;
      
      this.ticketCreated.emit();
    });
  }
}