import { Route } from '@angular/router';
import { DashboardPage } from './dashboard/dashboard.page';
import { HomePage } from './pages/home/home.page';

export const appRoutes: Route[] = [
	{ path: '', component: HomePage },
	{
		path: 'destinations/:id',
		loadComponent: () =>
			import('./pages/destination-detail/destination-detail.page').then(
				(module) => module.DestinationDetailPage,
			),
	},
	{
		path: 'trips',
		loadComponent: () => import('./trips/trips.page').then((module) => module.TripsPage),
	},
	{ path: 'dashboard', component: DashboardPage },
	{
		path: 'trips/:id',
		loadComponent: () =>
			import('./trips/trip-detail.page').then((module) => module.TripDetailPage),
	},
	{ path: '**', redirectTo: '' },
];
