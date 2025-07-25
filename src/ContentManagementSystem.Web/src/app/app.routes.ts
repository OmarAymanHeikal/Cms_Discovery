import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: '/discovery', pathMatch: 'full' },
  { path: 'discovery', loadComponent: () => import('./discovery/discovery.component').then(c => c.DiscoveryComponent) },
  { path: 'cms', loadComponent: () => import('./cms/cms.component').then(c => c.CmsComponent) },
  { path: '**', redirectTo: '/discovery' }
];