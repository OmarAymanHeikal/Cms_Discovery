import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-cms',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="cms-container">
      <h1>Content Management System</h1>
      <p class="lead">Manage your video content, categories, and metadata.</p>

      <!-- Action Buttons -->
      <div class="action-buttons mb-4">
        <button class="btn btn-primary me-2" (click)="showCreateForm()">
          ➕ Create New Program
        </button>
        <button class="btn btn-secondary me-2" (click)="loadPrograms()">
          🔄 Refresh List
        </button>
        <button class="btn btn-info" (click)="showStats()">
          📊 View Statistics
        </button>
      </div>

      <!-- Create/Edit Form -->
      <div class="program-form card" *ngIf="showForm">
        <div class="card-header">
          <h3>{{editMode ? 'Edit Program' : 'Create New Program'}}</h3>
          <button class="btn btn-sm btn-outline-secondary float-end" (click)="hideForm()">✕</button>
        </div>
        <div class="card-body">
          <form (ngSubmit)="saveProgram()">
            <div class="row">
              <div class="col-md-6">
                <div class="form-group mb-3">
                  <label>Title *</label>
                  <input type="text" class="form-control" [(ngModel)]="currentProgram.title" name="title" required>
                </div>
              </div>
              <div class="col-md-6">
                <div class="form-group mb-3">
                  <label>Type *</label>
                  <select class="form-control" [(ngModel)]="currentProgram.type" name="type" required>
                    <option value="1">Podcast</option>
                    <option value="2">Documentary</option>
                    <option value="3">Interview</option>
                    <option value="4">Tutorial</option>
                    <option value="5">News</option>
                  </select>
                </div>
              </div>
            </div>

            <div class="form-group mb-3">
              <label>Description</label>
              <textarea class="form-control" rows="3" [(ngModel)]="currentProgram.description" name="description"></textarea>
            </div>

            <div class="row">
              <div class="col-md-6">
                <div class="form-group mb-3">
                  <label>Video URL *</label>
                  <input type="url" class="form-control" [(ngModel)]="currentProgram.videoUrl" name="videoUrl" required>
                </div>
              </div>
              <div class="col-md-6">
                <div class="form-group mb-3">
                  <label>Thumbnail URL</label>
                  <input type="url" class="form-control" [(ngModel)]="currentProgram.thumbnailUrl" name="thumbnailUrl">
                </div>
              </div>
            </div>

            <div class="row">
              <div class="col-md-4">
                <div class="form-group mb-3">
                  <label>Duration (HH:MM:SS)</label>
                  <input type="text" class="form-control" [(ngModel)]="durationString" name="duration" placeholder="00:30:00">
                </div>
              </div>
              <div class="col-md-4">
                <div class="form-group mb-3">
                  <label>Language *</label>
                  <select class="form-control" [(ngModel)]="currentProgram.language" name="language" required>
                    <option value="1">Arabic</option>
                    <option value="2">English</option>
                    <option value="3">French</option>
                    <option value="4">Spanish</option>
                  </select>
                </div>
              </div>
              <div class="col-md-4">
                <div class="form-group mb-3">
                  <label>Status *</label>
                  <select class="form-control" [(ngModel)]="currentProgram.status" name="status" required>
                    <option value="1">Draft</option>
                    <option value="2">Under Review</option>
                    <option value="3">Published</option>
                    <option value="4">Archived</option>
                    <option value="5">Rejected</option>
                  </select>
                </div>
              </div>
            </div>

            <div class="form-group mb-3">
              <label>Published Date *</label>
              <input type="datetime-local" class="form-control" [(ngModel)]="publishedDateString" name="publishedDate" required>
            </div>

            <div class="form-actions">
              <button type="submit" class="btn btn-primary me-2">
                {{editMode ? 'Update Program' : 'Create Program'}}
              </button>
              <button type="button" class="btn btn-secondary" (click)="hideForm()">Cancel</button>
            </div>
          </form>
        </div>
      </div>

      <!-- Programs List -->
      <div class="programs-list">
        <h3>All Programs ({{totalCount}})</h3>
        
        <!-- Filters -->
        <div class="filters card mb-3">
          <div class="card-body">
            <div class="row">
              <div class="col-md-3">
                <select class="form-control" [(ngModel)]="filterStatus" (change)="loadPrograms()">
                  <option value="">All Status</option>
                  <option value="1">Draft</option>
                  <option value="2">Under Review</option>
                  <option value="3">Published</option>
                  <option value="4">Archived</option>
                  <option value="5">Rejected</option>
                </select>
              </div>
              <div class="col-md-3">
                <select class="form-control" [(ngModel)]="filterType" (change)="loadPrograms()">
                  <option value="">All Types</option>
                  <option value="1">Podcast</option>
                  <option value="2">Documentary</option>
                  <option value="3">Interview</option>
                  <option value="4">Tutorial</option>
                  <option value="5">News</option>
                </select>
              </div>
              <div class="col-md-4">
                <input type="text" class="form-control" placeholder="Search programs..." [(ngModel)]="searchTerm" (keyup.enter)="loadPrograms()">
              </div>
              <div class="col-md-2">
                <button class="btn btn-outline-primary w-100" (click)="loadPrograms()">Search</button>
              </div>
            </div>
          </div>
        </div>

        <!-- Programs Table -->
        <div class="table-responsive">
          <table class="table table-striped">
            <thead>
              <tr>
                <th>Title</th>
                <th>Type</th>
                <th>Language</th>
                <th>Status</th>
                <th>Views</th>
                <th>Created</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let program of programs">
                <td>
                  <strong>{{program.title}}</strong>
                  <br>
                  <small class="text-muted">{{program.description | slice:0:50}}{{program.description.length > 50 ? '...' : ''}}</small>
                </td>
                <td>
                  <span class="badge badge-info">{{program.typeName}}</span>
                </td>
                <td>{{program.languageName}}</td>
                <td>
                  <span class="badge" [ngClass]="getStatusClass(program.status)">
                    {{program.statusName}}
                  </span>
                </td>
                <td>{{program.viewCount}}</td>
                <td>{{program.createdAt | date:'short'}}</td>
                <td>
                  <button class="btn btn-sm btn-outline-primary me-1" (click)="editProgram(program)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" (click)="deleteProgram(program.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Loading State -->
        <div class="text-center" *ngIf="loading">
          <div class="loading"></div>
          <p>Loading programs...</p>
        </div>

        <!-- No Programs -->
        <div class="alert alert-info" *ngIf="programs.length === 0 && !loading">
          <h5>No programs found</h5>
          <p>Create your first program to get started!</p>
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
    </div>
  `,
  styles: [`
    .cms-container {
      max-width: 1400px;
      margin: 0 auto;
    }

    .action-buttons {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
    }

    .program-form {
      margin-bottom: 2rem;
    }

    .form-actions {
      border-top: 1px solid #dee2e6;
      padding-top: 1rem;
      margin-top: 1rem;
    }

    .table th {
      border-top: none;
      font-weight: 600;
    }

    .badge-success { background-color: #28a745; }
    .badge-warning { background-color: #ffc107; color: #212529; }
    .badge-danger { background-color: #dc3545; }
    .badge-secondary { background-color: #6c757d; }
    .badge-info { background-color: #17a2b8; }

    .loading {
      display: inline-block;
      width: 40px;
      height: 40px;
      border: 4px solid #f3f3f3;
      border-top: 4px solid #007bff;
      border-radius: 50%;
      animation: spin 1s linear infinite;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }

    .filters {
      background-color: #f8f9fa;
    }

    @media (max-width: 768px) {
      .action-buttons {
        flex-direction: column;
      }
      
      .table-responsive {
        font-size: 0.875rem;
      }
    }
  `]
})
export class CmsComponent implements OnInit {
  programs: any[] = [];
  currentProgram: any = this.getEmptyProgram();
  showForm = false;
  editMode = false;
  loading = false;
  
  // Pagination
  currentPage = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 0;
  
  // Filters
  filterStatus = '';
  filterType = '';
  searchTerm = '';
  
  // Form helpers
  durationString = '';
  publishedDateString = '';

  private apiUrl = 'https://localhost:7000/api/cms';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.loadPrograms();
  }

  getEmptyProgram() {
    return {
      title: '',
      description: '',
      thumbnailUrl: '',
      videoUrl: '',
      duration: '00:30:00',
      publishedDate: new Date().toISOString(),
      type: 1,
      language: 1,
      status: 1,
      categoryIds: [],
      tagIds: []
    };
  }

  showCreateForm() {
    this.currentProgram = this.getEmptyProgram();
    this.editMode = false;
    this.showForm = true;
    this.durationString = '00:30:00';
    this.publishedDateString = new Date().toISOString().slice(0, 16);
  }

  hideForm() {
    this.showForm = false;
    this.editMode = false;
    this.currentProgram = this.getEmptyProgram();
  }

  editProgram(program: any) {
    this.currentProgram = { ...program };
    this.editMode = true;
    this.showForm = true;
    this.durationString = this.formatDuration(program.duration);
    this.publishedDateString = new Date(program.publishedDate).toISOString().slice(0, 16);
  }

  saveProgram() {
    // Convert duration string to TimeSpan format
    this.currentProgram.duration = this.durationString;
    this.currentProgram.publishedDate = new Date(this.publishedDateString).toISOString();

    const request = this.editMode 
      ? this.http.put(`${this.apiUrl}/programs/${this.currentProgram.id}`, this.currentProgram)
      : this.http.post(`${this.apiUrl}/programs`, this.currentProgram);

    request.subscribe({
      next: (result) => {
        console.log('Program saved successfully:', result);
        this.hideForm();
        this.loadPrograms();
      },
      error: (error) => {
        console.error('Failed to save program:', error);
        alert('Failed to save program. Please try again.');
      }
    });
  }

  deleteProgram(id: string) {
    if (confirm('Are you sure you want to delete this program?')) {
      this.http.delete(`${this.apiUrl}/programs/${id}`).subscribe({
        next: () => {
          console.log('Program deleted successfully');
          this.loadPrograms();
        },
        error: (error) => {
          console.error('Failed to delete program:', error);
          alert('Failed to delete program. Please try again.');
        }
      });
    }
  }

  loadPrograms() {
    this.loading = true;
    
    const searchData = {
      searchTerm: this.searchTerm,
      type: this.filterType ? parseInt(this.filterType) : null,
      status: this.filterStatus ? parseInt(this.filterStatus) : null,
      page: this.currentPage,
      pageSize: this.pageSize
    };

    this.http.post(`${this.apiUrl}/programs/search`, searchData).subscribe({
      next: (result: any) => {
        this.programs = result.items;
        this.totalCount = result.totalCount;
        this.totalPages = Math.ceil(this.totalCount / this.pageSize);
        this.loading = false;
      },
      error: (error) => {
        console.error('Failed to load programs:', error);
        this.programs = [];
        this.loading = false;
      }
    });
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadPrograms();
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

  getStatusClass(status: number): string {
    switch (status) {
      case 1: return 'badge-secondary'; // Draft
      case 2: return 'badge-warning';   // Under Review
      case 3: return 'badge-success';   // Published
      case 4: return 'badge-info';      // Archived
      case 5: return 'badge-danger';    // Rejected
      default: return 'badge-secondary';
    }
  }

  formatDuration(duration: string): string {
    // Convert TimeSpan to HH:MM:SS format
    return duration || '00:30:00';
  }

  showStats() {
    alert('Statistics feature coming soon!');
  }
}