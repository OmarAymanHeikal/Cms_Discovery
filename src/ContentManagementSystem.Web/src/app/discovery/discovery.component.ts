import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-discovery',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="discovery-container">
      <h1>Discover Content</h1>
      <p class="lead">Explore our collection of podcasts, documentaries, and educational content.</p>

      <!-- Search Section -->
      <div class="search-section card">
        <div class="card-header">
          <h3>Search Content</h3>
        </div>
        <div class="card-body">
          <div class="row">
            <div class="col">
              <div class="form-group">
                <input 
                  type="text" 
                  class="form-control" 
                  placeholder="Search for programs..." 
                  [(ngModel)]="searchTerm">
              </div>
            </div>
            <div class="col-auto">
              <button class="btn btn-primary" (click)="search()">
                Search
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Links -->
      <div class="row mt-4">
        <div class="col-md-4">
          <div class="card">
            <div class="card-body">
              <h5>🔥 Trending Now</h5>
              <p>Most viewed content this week</p>
              <button class="btn btn-secondary" (click)="loadTrending()">View Trending</button>
            </div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card">
            <div class="card-body">
              <h5>🆕 Recently Added</h5>
              <p>Latest published content</p>
              <button class="btn btn-secondary" (click)="loadRecent()">View Recent</button>
            </div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card">
            <div class="card-body">
              <h5>📚 All Categories</h5>
              <p>Browse by category</p>
              <button class="btn btn-secondary">Browse Categories</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Results Section -->
      <div class="results-section mt-4" *ngIf="programs.length > 0">
        <h3>Search Results ({{totalCount}} found)</h3>
        <div class="row">
          <div class="col-md-6" *ngFor="let program of programs">
            <div class="card mb-3">
              <div class="card-body">
                <h5>{{program.title}}</h5>
                <p class="text-muted">{{program.description}}</p>
                <div class="program-meta">
                  <span class="badge badge-primary me-2">{{program.typeName}}</span>
                  <span class="badge badge-secondary me-2">{{program.languageName}}</span>
                  <span class="text-muted">👁️ {{program.viewCount}} views</span>
                </div>
                <div class="mt-2">
                  <button class="btn btn-outline-primary btn-sm">View Details</button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Pagination -->
        <nav *ngIf="totalPages > 1">
          <ul class="pagination justify-content-center">
            <li class="page-item" [class.disabled]="currentPage === 1">
              <button class="page-link" (click)="changePage(currentPage - 1)">Previous</button>
            </li>
            <li class="page-item" *ngFor="let page of getPageNumbers()" [class.active]="page === currentPage">
              <button class="page-link" (click)="changePage(page)">{{page}}</button>
            </li>
            <li class="page-item" [class.disabled]="currentPage === totalPages">
              <button class="page-link" (click)="changePage(currentPage + 1)">Next</button>
            </li>
          </ul>
        </nav>
      </div>

      <!-- Loading State -->
      <div class="loading text-center" *ngIf="loading">
        <p>Loading content...</p>
      </div>

      <!-- No Results -->
      <div class="alert alert-info" *ngIf="searched && programs.length === 0 && !loading">
        <h5>No content found</h5>
        <p>Try adjusting your search terms or browse by category.</p>
      </div>
    </div>
  `,
  styles: [`
    .discovery-container {
      max-width: 1200px;
      margin: 0 auto;
    }

    .search-section {
      margin-bottom: 2rem;
    }

    .program-meta {
      margin-top: 1rem;
    }

    .badge {
      margin-right: 0.5rem;
    }

    .pagination {
      margin-top: 2rem;
    }

    .card {
      transition: transform 0.2s ease;
    }

    .card:hover {
      transform: translateY(-2px);
    }

    .loading::after {
      content: '';
      display: inline-block;
      width: 20px;
      height: 20px;
      border: 3px solid #f3f3f3;
      border-top: 3px solid #007bff;
      border-radius: 50%;
      animation: spin 1s linear infinite;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }
  `]
})
export class DiscoveryComponent implements OnInit {
  searchTerm = '';
  programs: any[] = [];
  loading = false;
  searched = false;
  currentPage = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 0;

  private apiUrl = 'https://localhost:7000/api/discovery';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.loadRecent();
  }

  search() {
    this.loading = true;
    this.searched = true;
    
    const params = {
      searchTerm: this.searchTerm,
      page: this.currentPage,
      pageSize: this.pageSize
    };

    this.http.get(`${this.apiUrl}/search`, { params }).subscribe({
      next: (result: any) => {
        this.programs = result.items;
        this.totalCount = result.totalCount;
        this.totalPages = Math.ceil(this.totalCount / this.pageSize);
        this.loading = false;
      },
      error: (error) => {
        console.error('Search failed:', error);
        this.programs = [];
        this.loading = false;
      }
    });
  }

  loadTrending() {
    this.loading = true;
    this.http.get(`${this.apiUrl}/trending?count=10`).subscribe({
      next: (programs: any) => {
        this.programs = programs;
        this.loading = false;
        this.searched = true;
      },
      error: (error) => {
        console.error('Failed to load trending:', error);
        this.loading = false;
      }
    });
  }

  loadRecent() {
    this.loading = true;
    this.http.get(`${this.apiUrl}/recent?count=10`).subscribe({
      next: (programs: any) => {
        this.programs = programs;
        this.loading = false;
        this.searched = true;
      },
      error: (error) => {
        console.error('Failed to load recent:', error);
        this.loading = false;
      }
    });
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.search();
    }
  }

  getPageNumbers(): number[] {
    const pages = [];
    const start = Math.max(1, this.currentPage - 2);
    const end = Math.min(this.totalPages, this.currentPage + 2);
    
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    
    return pages;
  }
}