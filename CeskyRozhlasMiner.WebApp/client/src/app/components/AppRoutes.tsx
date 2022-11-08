import About from "app/pages/about/About";
import Home from "app/pages/home/Home";
import PlaylistInfo from "app/pages/playlists/PlaylistInfo";
import Playlists from "app/pages/playlists/Playlists";
import Songs from "app/pages/songs/Songs";
import UserManage from "app/pages/user/UserManage";
import UserSignIn from "app/pages/user/UserSignIn";
import UserSignOut from "app/pages/user/UserSignOut";

import { appAlertsAtom } from "app/state/atom";
import React, { useEffect } from "react";
import { Route, Routes, useLocation } from "react-router-dom";
import { useSetRecoilState } from "recoil";
import TokenVerifyUser from "./token/TokenVerifyUser";


export enum AppRouteGroup {
    Default = 0,
    User = 1,
};

export enum AppRoute {
    Default = 0,
    Home = 1,
    About = 2,
    Playlists = 3,
    UserSignOut = 4,
    UserSignIn = 5,
    UserSettings = 6,
    Songs = 7,
    PlaylistInfo = 8,
    UserSignUp = 9,
    TokenVerifyUser = 10,
};

export interface AppRouteDefinition {
    path: string;
    element: React.ReactElement;
    inMenu: boolean;
    title: string;
    menuTitle: string|null;
    beAuthorized: boolean;
    group: AppRouteGroup,
};

export const UseRoutes : Record<number, AppRouteDefinition> = {};

UseRoutes[AppRoute.Default] = {
    path: '/',
    element: <Home />,
    inMenu: false,
    title: 'Home',
    menuTitle: 'Home',
    beAuthorized: false,
    group: AppRouteGroup.Default,
};

UseRoutes[AppRoute.Home] = {
    path: '/home',
    element: UseRoutes[AppRoute.Default].element,
    inMenu: true,
    title: UseRoutes[AppRoute.Default].title,
    menuTitle: UseRoutes[AppRoute.Default].menuTitle,
    beAuthorized: false,
    group: AppRouteGroup.Default,
};

UseRoutes[AppRoute.About] = {
    path: '/about',
    element: <About />,
    inMenu: true,
    title: 'About',
    menuTitle: 'About',
    beAuthorized: false,
    group: AppRouteGroup.Default,
};

UseRoutes[AppRoute.Playlists] = {
    path: '/playlists',
    element: <Playlists />,
    inMenu: true,
    title: 'Playlists',
    menuTitle: 'Playlists',
    beAuthorized: true,
    group: AppRouteGroup.Default,
};

UseRoutes[AppRoute.UserSignIn] = {
    path: '/user/sign-in',
    element: <UserSignIn />,
    inMenu: true,
    title: 'Sign in',
    menuTitle: 'Sign in',
    beAuthorized: false,
    group: AppRouteGroup.User,
};

UseRoutes[AppRoute.UserSignUp] = {
    path: '/user/sign-up',
    element: <UserManage />,
    inMenu: true,
    title: 'Sign up',
    menuTitle: 'Sign up',
    beAuthorized: false,
    group: AppRouteGroup.User,
};

UseRoutes[AppRoute.UserSignOut] = {
    path: '/user/sign-out',
    element: <UserSignOut />,
    inMenu: true,
    title: 'Sign out',
    menuTitle: 'Sign out',
    beAuthorized: true,
    group: AppRouteGroup.User,
};

UseRoutes[AppRoute.UserSettings] = {
    path: '/user/settings',
    element: <UserManage />,
    inMenu: true,
    title: 'User settings',
    menuTitle: 'Settings',
    beAuthorized: true,
    group: AppRouteGroup.User,
};

UseRoutes[AppRoute.Songs] = {
    path: '/songs',
    element: <Songs />,
    inMenu: false,
    title: 'Songs',
    menuTitle: 'Songs',
    beAuthorized: true,
    group: AppRouteGroup.Default,
};

UseRoutes[AppRoute.PlaylistInfo] = {
    path: '/playlist-info',
    element: <PlaylistInfo />,
    inMenu: false,
    title: 'Playlist information',
    menuTitle: 'Playlist information',
    beAuthorized: true,
    group: AppRouteGroup.Default,
};

UseRoutes[AppRoute.TokenVerifyUser] = {
    path: '/token/verify-user',
    element: <TokenVerifyUser />,
    inMenu: false,
    title: 'Verify user',
    menuTitle: 'Verify user',
    beAuthorized: false,
    group: AppRouteGroup.Default,
};


export const iterateThroughRoutes = (): AppRoute[] => {
    return Object.keys(UseRoutes)
    .filter(route => !isNaN(Number(route)))
    .map(route => Number(route) as AppRoute);
};

export interface AppRoutesProps {
    onChange: (route: AppRoute) => void;
}

const AppRoutes: React.FC<AppRoutesProps> = ({onChange}: AppRoutesProps) => {
    const setAppAlerts = useSetRecoilState(appAlertsAtom);
    const location = useLocation();

    useEffect(() => {
        const route = Object.keys(UseRoutes).map(k => k as unknown as AppRoute)
            .find(r => UseRoutes[r as AppRoute].path === location.pathname) ?? AppRoute.Default;

        onChange(route);

        setAppAlerts([]);
    }, [location, onChange, setAppAlerts]);

    return <Routes>
        {iterateThroughRoutes().map(route => {
            return <Route
                key={route}
                path={UseRoutes[route].path}
                element={UseRoutes[route].element}
            />
        })}
    </Routes>
};

export default AppRoutes;