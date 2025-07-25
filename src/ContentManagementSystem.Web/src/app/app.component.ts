import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterModule } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterModule],
  template: `
    <nav class="navbar">
      <div class="container">
        <a class="navbar-brand" routerLink="/">
          <h2>Content Management System</h2>
        </a>
        <ul class="navbar-nav">
          <li class="nav-item">
            <a class="nav-link" routerLink="/discovery" routerLinkActive="active">Discovery</a>
          </li>
          <li class="nav-item">
            <a class="nav-link" routerLink="/cms" routerLinkActive="active">CMS</a>
          </li>
        </ul>
      </div>
    </nav>

    <main class="container mt-4">
      <router-outlet></router-outlet>
    </main>

    <footer class="mt-5 p-4 text-center" style="background-color: #f8f9fa;">
      <p>&copy; 2024 Content Management & Discovery System - Thamania</p>
    </footer>
  `,
  styles: [`
    .navbar {
      background-color: white;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      padding: 1rem 0;
      margin-bottom: 2rem;
    }
    
    .navbar .container {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    
    .navbar-brand h2 {
      margin: 0;
      color: #007bff;
      font-weight: 700;
    }
    
    .navbar-nav {
      display: flex;
      list-style: none;
      margin: 0;
      padding: 0;
    }
    
    .nav-item {
      margin-left: 2rem;
    }
    
    .nav-link {
      color: #343a40;
      text-decoration: none;
      font-weight: 500;
      padding: 0.5rem 1rem;
      border-radius: 4px;
      transition: all 0.3s ease;
    }
    
    .nav-link:hover,
    .nav-link.active {
      color: #007bff;
      background-color: rgba(0, 123, 255, 0.1);
    }
    
    main {
      min-height: 70vh;
    }
  `]
})
export class AppComponent {
  title = 'Content Management System';
}