import React from 'react';
import { Typography, Box, Button, ButtonGroup } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { AppRoute, UseRoutes } from 'app/components/AppRoutes';
import CurrentlyPlaying from 'app/components/songs/CurrentlyPlaying';
import { useRecoilValue } from 'recoil';
import { userAtom } from 'app/state/atom';
import { AppRegistration, ArrowForward, List as ListIcon, Login, Person } from '@mui/icons-material';

const Home: React.FC = () => {
    const navigate = useNavigate();
    const user = useRecoilValue(userAtom);

    return (
        <Box>
            <Typography variant="h5" component="h2" color="primary.main">
                Welcome{user === null ? null : ` ${user.displayName}`}
            </Typography>
            <Box mb={3}>
                <Box display="inline-block" textAlign={{ md: 'right' }}>
                    <Box mb={2}>
                        <Typography variant="body1" component="p">
                            Let's see what's playing in Cesky rozhlas radio stations across Czech Republic.
                        </Typography>
                    </Box>
                    <Button onClick={() => navigate(UseRoutes[AppRoute.About].path)} variant="contained" color="info">
                        About app <ArrowForward />
                    </Button>
                </Box>
            </Box>

            <CurrentlyPlaying updateIntervalSeconds={60} />

            {user === null ? (
                <ButtonGroup>
                    <Button onClick={() => navigate(UseRoutes[AppRoute.UserSignIn].path)} variant="contained" color="primary">
                        <Login /> Sign in
                    </Button>
                    <Button onClick={() => navigate(UseRoutes[AppRoute.UserSignUp].path)} variant="contained" color="secondary">
                        <Person /> Sign up
                    </Button>
                </ButtonGroup>
            ) : (
                <Button
                    onClick={() => navigate(UseRoutes[AppRoute.Playlists].path)}
                    variant="contained"
                    color={'dark' as 'inherit'}>
                    <ListIcon /> Your playlists
                </Button>
            )}
        </Box>
    );
};

export default Home;
