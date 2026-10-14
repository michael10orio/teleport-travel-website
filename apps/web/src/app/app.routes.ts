import { Route } from '@angular/router';
import { HomePage } from './pages/home/home.page';

export const appRoutes: Route[] = [
	{ path: '', component: HomePage },
	{
		path: 'trips',
		loadComponent: () => import('./trips/trips.page').then((module) => module.TripsPage),
	},
	{
		path: 'trips/:id',
		loadComponent: () =>
			import('./trips/trip-detail.page').then((module) => module.TripDetailPage),
	},
	{ path: '**', redirectTo: '' },
];
