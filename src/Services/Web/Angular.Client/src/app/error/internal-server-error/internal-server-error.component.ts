import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-internal-server-error',
  standalone: false,
  templateUrl: './internal-server-error.component.html',
  styleUrl: './internal-server-error.component.scss'
})
export class InternalServerErrorComponent {
  constructor(private router: Router) { }

  goHome() {
    this.router.navigate(['/']);
  }
}
