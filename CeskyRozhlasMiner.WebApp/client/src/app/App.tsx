import React, { useEffect, useState } from 'react';
import { BrowserRouter } from 'react-router-dom';

import * as serviceWorker from '../serviceWorker';
import AppResponsiveBar from './components/AppResponsiveBar';

import { createTheme, ThemeProvider } from '@mui/material/styles';
import './App.module.scss';
import { Box, Typography } from '@mui/material';
import AppRoutes, { AppRoute, UseRoutes } from './components/AppRoutes';

import { Alert, IconButton } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';

import { useRecoilState, useSetRecoilState } from 'recoil';
import { appAlertsAtom, userAtom } from './state/atom';
import AppWrapper from './components/AppWrapper';
import { ApiClient, ApiException } from './generated/backend';
import { LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterMoment } from '@mui/x-date-pickers/AdapterMoment';

import { GoogleOAuthProvider } from '@react-oauth/google';
import { getErrorMessage } from './utils/utilities';

export interface AppProps {
    brand: string;
    authors: string[];
}

const App: React.FC<AppProps> = ({ brand, authors }: AppProps) => {
    const [appAlerts, setAppAlerts] = useRecoilState(appAlertsAtom);
    const [activeRoute, setActiveRoute] = useState<AppRoute>(AppRoute.Default);
    const setUser = useSetRecoilState(userAtom);
    const [googleClientId, setGoogleClientId] = useState<string>('');

    useEffect(() => {
        const fetchUser = async () => {
            try {
                setUser(await new ApiClient(process.env.REACT_APP_API_BASE).users_GetUser());
            } catch (_) {}
        };

        fetchUser();

        const fetchGoogleClientId = async () => {
            try {
                setGoogleClientId(await new ApiClient(process.env.REACT_APP_API_BASE).google_GetClientId());
            } catch (e) {
                console.log(e);
                setAppAlerts([
                    ...appAlerts,
                    {
                        text: getErrorMessage(e as ApiException),
                        severity: 'error',
                    }
                ]);
            }
        };

        fetchGoogleClientId();
    // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    useEffect(() => {
        document.title = `${UseRoutes[activeRoute].title} - ${brand}`;
    }, [activeRoute, brand]);

    const theme = createTheme({
        palette: {
            primary: {
                main: '#0157aa'
            }
        }
    });

    return (
        <BrowserRouter>
            <ThemeProvider theme={theme}>
                <LocalizationProvider dateAdapter={AdapterMoment}>
                    <GoogleOAuthProvider clientId={googleClientId}>
                        <AppResponsiveBar />
                        <Box component="main">
                            <AppWrapper pt={3} pb={5} my={2} textAlign={{ xs: 'center', md: 'left' }}>
                                <Box component="div" mb={appAlerts.length === 0 ? 0 : 3}>
                                    {appAlerts.map((alert, i) => (
                                        <Box
                                            key={i}
                                            component={Alert}
                                            severity={alert.severity}
                                            action={
                                                <IconButton
                                                    aria-label="close"
                                                    color="inherit"
                                                    size="small"
                                                    onClick={() => {
                                                        setAppAlerts(appAlerts.filter((_, inx) => inx !== i));
                                                    }}>
                                                    <CloseIcon fontSize="inherit" />
                                                </IconButton>
                                            }
                                            mb={1}>
                                            {alert.text}
                                        </Box>
                                    ))}
                                </Box>
                                <Box mb={2}>
                                    <Typography component="h1" variant="h4">
                                        {UseRoutes[activeRoute].title}
                                    </Typography>
                                </Box>
                                <AppRoutes onChange={setActiveRoute} />
                            </AppWrapper>
                        </Box>
                        <Box
                            component="footer"
                            position="fixed"
                            bottom={0}
                            width="100%"
                            py={2}
                            px={3}
                            boxSizing="border-box"
                            bgcolor="#fff">
                            <Typography variant="subtitle1" color="text.secondary" align="center">
                                @ {authors.join(', ')} {new Date().getFullYear()}, All rights reserved
                            </Typography>
                        </Box>
                    </GoogleOAuthProvider>
                </LocalizationProvider>
            </ThemeProvider>
        </BrowserRouter>
    );
};

export default App;

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
