import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TicketList } from './components/ticket-list/ticket-list';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, TicketList],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('TicketFlow.Web');
}
