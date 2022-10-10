import { Box, Avatar, TextField, Button, Grid, Link } from '@mui/material';
import { LockOutlined } from '@mui/icons-material';

import React, { useState } from 'react';
import { appAlertsAtom, userAtom } from 'app/state/atom';
import { useSetRecoilState } from 'recoil';
import { ApiClient, ApiException, GoogleSignInDataDto, IUserAuthenticateDto, UserAuthenticateDto } from 'app/generated/backend';
import { getErrorMessage } from 'app/utils/utilities';
import { useNavigate } from 'react-router-dom';
import { AppRoute, UseRoutes } from 'app/components/AppRoutes';
import { GoogleLogin } from '@react-oauth/google';
import dayjs from 'app/utils/dayjsAsUtc';

const UserSignIn: React.FC = () => {
    const setAppAlerts = useSetRecoilState(appAlertsAtom);
    const setUser = useSetRecoilState(userAtom);
    const navigate = useNavigate();
    
    const [signingIn, setSigningIn] = useState<boolean>(false);
    const now = dayjs();
    const [userAuthenticate, setUserAuthenticate] = useState<IUserAuthenticateDto>({
        email: '',
        password: '',
        createdDate: now as unknown as Date,
        updatedDate: now as unknown as Date,
        id: 0,
    });

    const handleGoogleSignIn = async (dto: GoogleSignInDataDto) => {
      try {
        setSigningIn(true);
        setUser(await new ApiClient(process.env.REACT_APP_API_BASE).google_SignInUser(dto));
        setSigningIn(false);
        navigate(UseRoutes[AppRoute.UserSettings].path);
      } catch (e) {
          console.log(e);
          setAppAlerts((appAlerts) => [
            ...appAlerts,
            {
              text: getErrorMessage(e as ApiException),
              severity: 'error',
            }
          ]);
          setSigningIn(false);
      }
    };

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        const fetchData = async () => {
            try {
                const data = new UserAuthenticateDto();
                data.init(userAuthenticate);

                setSigningIn(true);
                setUser(await new ApiClient(process.env.REACT_APP_API_BASE).users_SignInUser(data));
                setSigningIn(false);

                navigate(UseRoutes[AppRoute.UserSettings].path);
            } catch (e) {
                console.log(e);
                setAppAlerts((appAlerts) => [
                  ...appAlerts,
                  {
                    text: getErrorMessage(e as ApiException),
                    severity: 'error',
                  }
                ]);
                setSigningIn(false);
                setUserAuthenticate({
                  ...userAuthenticate,
                  password: '',
                });
            }
        };

        fetchData();
    };

    return <Grid container>
        <Grid item md={7} lg={5} xl={4}>
            <Box mb={2} sx={{ display: 'flex' }} justifyContent="center">
                <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
                  <LockOutlined />
                </Avatar>
            </Box>
            <Box component="form" onSubmit={handleSubmit} noValidate mt={1} mb={4}>
              <TextField
                margin="normal"
                required
                fullWidth
                id="email"
                label="Email Address"
                name="email"
                autoComplete="email"
                autoFocus
                value={userAuthenticate.email}
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
                value={userAuthenticate.password}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => setUserAuthenticate({
                    ...userAuthenticate,
                    password: e.currentTarget.value,
                })}
              />
              <Button
                type="submit"
                fullWidth
                variant="contained"
                sx={{ mt: 3, mb: 2 }}
                disabled={signingIn}
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
            
            <Box display="flex" justifyContent={{ xs: "center", md: "flex-end" }}>
              <GoogleLogin
                onSuccess={credentialResponse => {
                  const data = new GoogleSignInDataDto();
                  data.init(credentialResponse);
                  handleGoogleSignIn(data);
                }}
                onError={() => {
                  setAppAlerts((appAlerts) => [
                    ...appAlerts,
                    {
                      text: 'Login via Google failed',
                      severity: 'error',
                    }
                  ]);
                }}
                useOneTap
              />
            </Box>
        </Grid>
    </Grid>;
};

export default UserSignIn;