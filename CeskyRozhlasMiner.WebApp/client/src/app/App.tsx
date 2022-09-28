import React, { useEffect } from 'react';
import { BrowserRouter, Route, Routes } from 'react-router-dom';

import * as serviceWorker from '../serviceWorker';
import ResponsiveBar from './components/ResponsiveBar';

import { createTheme, ThemeProvider } from '@mui/material/styles';
import './App.module.scss';
import { Box, Typography } from '@mui/material';
import { AppRoutes, iterateThroughRoutes } from './routing/appRoutes';

import { Alert, IconButton, Grid } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';

import { useRecoilState } from 'recoil';
import { appAlertsAtom } from './state/atom';

export interface AppProps {
    brand: string;
    authors: string[];
};

const App: React.FC<AppProps> = ({brand, authors}: AppProps) => {
    const [appAlerts, setAppAlerts] = useRecoilState(appAlertsAtom);

    useEffect(() => {
        document.title = brand;
    });

    const theme = createTheme({
      palette: {
        primary: {
          main: '#0157aa',
        },
        secondary: {
          main: '#6c757d',
        }
      },
    });

    return (
        <BrowserRouter>
            <ThemeProvider theme={theme}>
                <ResponsiveBar />
                <Box component={Grid} container px={4} pt={3} pb={5} mb={2} justifyContent="center">
                    <Grid item lg={8}>
                        <Box component="div" mb={appAlerts.length === 0 ? 0 : 3}>
                            {appAlerts.map((alert, i) => 
                                <Box
                                    key={i}
                                    component={Alert}
                                    severity={alert.severity}
                                    action={
                                      <IconButton aria-label="close" color="inherit" size="small"
                                        onClick={() => {
                                          setAppAlerts(appAlerts.filter((_, inx) => inx !== i));
                                        }}
                                      >
                                        <CloseIcon fontSize="inherit" />
                                      </IconButton>
                                    }
                                    mb={1}
                                >
                                    {alert.text}
                                </Box>
                            )}
                        </Box>
                        <Routes>
                            {iterateThroughRoutes().map(route => {
                                return <Route
                                    key={route}
                                    path={AppRoutes[route].path}
                                    element={AppRoutes[route].element}
                                />
                            })}
                        </Routes>
                    </Grid>
                </Box>
                <Box component="div" position="fixed" bottom={0} width="100%" py={2} px={3} boxSizing="border-box">
                    <Typography variant="subtitle1" color="secondary" align="center">
                        @{new Date().getFullYear()}, {authors.join(', ')}, All rights reserved
                    </Typography>
                </Box>
            </ThemeProvider>
        </BrowserRouter>
    );
};

export default App;

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
