import About from "app/pages/about/About";
import Groups from "app/pages/groups/Groups";
import Home from "app/pages/home/Home";
import React from "react";

export enum AppRoute {
    Default = 0,
    Home = 1,
    About = 2,
    Groups = 3,
};

export interface AppRouteDefinition {
    path: string;
    element: React.ReactElement;
    inMenu: boolean;
    menuTitle: string|null;
};

export const AppRoutes : Record<number, AppRouteDefinition> = {};

AppRoutes[AppRoute.Default] = {
    path: '/',
    element: <Home />,
    inMenu: false,
    menuTitle: null,
};
AppRoutes[AppRoute.Home] = {
    path: '/home',
    element: AppRoutes[AppRoute.Default].element,
    inMenu: true,
    menuTitle: 'Home',
};
AppRoutes[AppRoute.About] = {
    path: '/about',
    element: <About />,
    inMenu: true,
    menuTitle: 'About',
};
AppRoutes[AppRoute.Groups] = {
    path: '/groups',
    element: <Groups />,
    inMenu: true,
    menuTitle: 'Groups',
};

export const iterateThroughRoutes = (): AppRoute[] => {
    return Object.keys(AppRoutes)
    .filter(route => !isNaN(Number(route)))
    .map(route => Number(route) as AppRoute);
};
