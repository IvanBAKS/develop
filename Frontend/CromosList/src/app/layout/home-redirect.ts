import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-home-redirect',
  template: '',
  imports: []
})
export class HomeRedirectComponent implements OnInit {
  private router = inject(Router);
  private auth = inject(AuthService);

  ngOnInit(): void {
    this.router.navigate([this.auth.isAdmin() ? '/panel' : '/colecciones']);
  }
}