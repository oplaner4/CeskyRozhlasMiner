import React from 'react';
import { Typography, Box, Button } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { AppRoute, UseRoutes } from 'app/components/AppRoutes';

const Home: React.FC = () => {
    const navigate = useNavigate();

    return <Box>
        <Typography component={Box} variant="body1" mb={2}>Welcome my friend</Typography>
        <Button onClick={() => navigate(UseRoutes[AppRoute.UserSignIn].path)} variant="contained" color="info">
            Sign in
        </Button>
    </Box>;
};

export default Home;
