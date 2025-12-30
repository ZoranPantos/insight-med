import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive], // RouterLink allows the <a> tags to work
  template: `
    <div class="app-container">
      <nav class="navbar">
        <div class="nav-group left">
          <a routerLink="/reports" routerLinkActive="active-link">Reports</a>
          <a routerLink="/requests" routerLinkActive="active-link">Requests</a>
          <a routerLink="/patients" routerLinkActive="active-link">Patients</a>
        </div>

        <div class="nav-group center">
          <input type="text" placeholder="Search..." />
          <button>Go</button>
        </div>

        <div class="nav-group right">
          <span>Notifications</span>
          <a routerLink="/profile" routerLinkActive="active-link">Profile</a>
        </div>
      </nav>

      <hr />

      <main>
        <router-outlet /> 
      </main>
    </div>
  `,
  // We reuse the styles you already liked, but we must link them here or copy them.
  // For simplicity, we will copy the specific layout CSS here in styles.
  styles: [`
  .app-container { width: 66%; margin: 0 auto; min-width: 800px; }
  
  .navbar { 
    display: flex; 
    justify-content: space-between; 
    align-items: center; 
    padding: 15px 0; 
  }
  
  .nav-group { 
    display: flex; 
    align-items: center; 
    gap: 10px; /* Reduced gap slightly since links are now wider due to padding */
  }

  /* BASE LINK STYLES */
  a { 
    text-decoration: none; 
    color: black; 
    font-weight: 500; 
    cursor: pointer;
    
    /* New: visual structure for the link */
    padding: 8px 16px;       /* Makes the clickable area bigger */
    border-radius: 4px;      /* Rounds the corners */
    transition: all 0.2s;    /* Smooth animation when clicking */
  }

  a:hover { 
    background-color: #e6e6e6; /* Light gray when hovering */
    text-decoration: none;     /* Remove underline (cleaner look) */
  }

  /* ACTIVE STATE STYLE */
  /* Angular applies this class automatically when the route matches! */
  .active-link {
    background-color: #0078d4; /* Deep Blue */
    color: white !important;   /* White text (forces override) */
  }
`]
})
export class MainLayoutComponent {}