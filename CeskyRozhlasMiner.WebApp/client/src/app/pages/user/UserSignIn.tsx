import { Box, Avatar, TextField, FormControlLabel, Checkbox, Button, Grid, Link } from '@mui/material';
import { LockOutlined } from '@mui/icons-material';

import React, { useState } from 'react';
import { appAlertsAtom, userAtom } from 'app/state/atom';
import { useSetRecoilState } from 'recoil';
import { ApiClient, ApiException, IUserAuthenticateDto, UserAuthenticateDto } from 'app/generated/backend';
import { getErrorMessage } from 'app/utils/utilities';
import { useNavigate } from 'react-router-dom';
import { AppRoute, UseRoutes } from 'app/components/AppRoutes';

const UserSignIn: React.FC = () => {
    const setAppAlerts = useSetRecoilState(appAlertsAtom);
    const setUser = useSetRecoilState(userAtom);
    const navigate = useNavigate();

    const [userAuthenticate, setUserAuthenticate] = useState<IUserAuthenticateDto>({
        email: '',
        password: '',
        createdDate: new Date(),
        updatedDate: new Date(),
        id: 0,
    });

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        const fetchData = async () => {
            try {
                const data = new UserAuthenticateDto();
                data.email = userAuthenticate.email;
                data.password = userAuthenticate.password;
                setUser(await new ApiClient(process.env.REACT_APP_API_BASE).users_SignInUser(data));
                navigate(UseRoutes[AppRoute.UserSettings].path);
                setAppAlerts([]);
            } catch (e) {
                console.log(e);

                setAppAlerts((appAlerts) => [
                  ...appAlerts,
                  {
                    text: getErrorMessage(e as ApiException),
                    severity: 'error',
                  }
                ]);
            }
        };

        fetchData();
    };

    return <Grid container>
        <Grid item md={8} lg={6}>
            <Box mb={2} sx={{ display: 'flex' }} justifyContent="center">
                <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
                  <LockOutlined />
                </Avatar>
            </Box>
            <Box component="form" onSubmit={handleSubmit} noValidate mt={1}>
              <TextField
                margin="normal"
                required
                fullWidth
                id="email"
                label="Email Address"
                name="email"
                autoComplete="email"
                autoFocus
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => setUserAuthenticate({
                    ...userAuthenticate,
                    email: e.currentTarget.value,
                })}
              />
              <TextField
                margin="normal"
                required
                fullWidth
                name="password"
                label="Password"
                type="password"
                id="password"
                autoComplete="current-password"
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => setUserAuthenticate({
                    ...userAuthenticate,
                    password: e.currentTarget.value,
                })}
              />
              <FormControlLabel
                control={<Checkbox value="remember" color="primary" />}
                label="Remember me"
              />
              <Button
                type="submit"
                fullWidth
                variant="contained"
                sx={{ mt: 3, mb: 2 }}
              >
                Sign In
              </Button>
              <Grid container>
                <Grid item xs>
                  <Link href="#" variant="body2">
                    Forgot password?
                  </Link>
                </Grid>
                <Grid item>
                  <Link href="#" variant="body2">
                    {"Don't have an account? Sign Up"}
                  </Link>
                </Grid>
              </Grid>
            </Box>
        </Grid>
    </Grid>;
};

export default UserSignIn;