import { Box, Button, FormGroup, Grid, TextField, Tooltip, Typography } from '@mui/material';
import { userAtom } from 'app/state/atom';
import React, { useEffect } from 'react';
import { useRecoilState, useSetRecoilState } from 'recoil';

import { ApiClient, ApiException, IUserSetDto, UserSetDto } from 'app/generated/backend';
import { appAlertsAtom } from 'app/state/atom';

import { useState } from 'react';

import { useNavigate } from 'react-router-dom';
import { AppRoute, UseRoutes } from 'app/components/AppRoutes';
import { Home } from '@mui/icons-material';
import FormattedErrorMessage from 'app/components/FormattedErrorMessage';

const UserManage: React.FC = () => {
    const [user, setUser] = useRecoilState(userAtom);

    const [data, setData] = useState<IUserSetDto>({
        id: 0,
        email: '',
        password: '',
        createdDate: new Date(),
        updatedDate: new Date(),
        newPasswordConfirm: '',
        newPassword: '',
        displayName: '',
        verified: false,
    });

    useEffect(() => {
        if (user !== null) {
            setData({
                ...user,
                newPasswordConfirm: '',
                newPassword: ''
            });
        }
    }, [user]);

    const setAppAlerts = useSetRecoilState(appAlertsAtom);

    const [saving, setSaving] = useState<boolean>(false);

    const navigate = useNavigate();

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        const saveData = async () => {
            try {
                setSaving(true);
                let saveData = new UserSetDto();
                saveData.init(data);

                const creating = saveData.id === 0;

                if (creating) {
                    saveData = await new ApiClient(process.env.REACT_APP_API_BASE).users_CreateUser(saveData);
                    navigate(UseRoutes[AppRoute.UserSignIn].path);
                } else {
                    saveData = await new ApiClient(process.env.REACT_APP_API_BASE).users_UpdateUser(saveData);
                }

                setUser(saveData);
                setAppAlerts((appAlerts) => [
                    ...appAlerts,
                    {
                        severity: 'success',
                        text: `Successfully ${creating ? 'created' : 'edited'}.`
                    }
                ]);

                setSaving(false);
            } catch (e) {
                console.log(e);
                setAppAlerts((appAlerts) => [
                    ...appAlerts,
                    {
                        severity: 'error',
                        text: <FormattedErrorMessage exception={e as ApiException} />
                    }
                ]);
                setSaving(false);
            }

            setData({
                ...data,
                password: '',
                newPassword: '',
                newPasswordConfirm: ''
            });
        };

        saveData();
    };


    const handleVerifyUser = async () => {
        try {
            await new ApiClient(process.env.REACT_APP_API_BASE).tokens_SendNewToken();
            setAppAlerts((appAlerts) => [
                ...appAlerts,
                {
                    severity: 'success',
                    text: <>Complete verification by opening link which was sent to <Typography component="span" fontWeight="bold">{data.email}</Typography>.</>,
                }
            ]);
        } catch (e) {
            console.log(e);
            setAppAlerts((appAlerts) => [
                ...appAlerts,
                {
                    severity: 'error',
                    text: <FormattedErrorMessage exception={e as ApiException} />
                }
            ]);
        }
    }

    return (
        <Box>
            <Grid container>
                <Grid item lg={6} xl={4} mb={3}>
                    <Box component="form" onSubmit={handleSubmit} noValidate mt={1}>
                        <Box component={FormGroup} mb={2}>
                            <TextField
                                margin="normal"
                                fullWidth
                                label="Id"
                                name="id"
                                value={data.id}
                                InputProps={{
                                    readOnly: true
                                }}
                                variant="filled"
                            />

                            <TextField
                                margin="normal"
                                fullWidth
                                label="Email adress"
                                name="email"
                                value={data.email}
                                InputProps={{
                                    readOnly: data.id !== 0
                                }}
                                required={data.id === 0}
                                autoFocus={data.id === 0}
                                variant={data.id === 0 ? 'outlined' : 'filled'}
                                onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                                    setData({
                                        ...data,
                                        email: e.currentTarget.value
                                    })
                                }
                            />

                            <TextField
                                margin="normal"
                                required
                                fullWidth
                                label="Name"
                                name="name"
                                autoFocus={data.id !== 0}
                                value={data.displayName}
                                onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                                    setData({
                                        ...data,
                                        displayName: e.currentTarget.value
                                    })
                                }
                            />
                        </Box>

                        {data.id === 0 ? null : (
                            <Box component={FormGroup} mb={3}>
                                <TextField
                                    margin="normal"
                                    required
                                    fullWidth
                                    label="Old password"
                                    name="password"
                                    type="password"
                                    value={data.password}
                                    onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                                        setData({
                                            ...data,
                                            password: e.currentTarget.value
                                        })
                                    }
                                />
                            </Box>
                        )}

                        {data.id === 0 || data.verified ? null : (
                            <Box component={FormGroup} mb={3}>
                                <Typography component="p" variant="body2" color="warning.main">
                                    By verifying your account you can later reset forgotten password and 
                                    use all features of this application.
                                </Typography>
                                <Button variant="contained" color="warning" onClick={handleVerifyUser} sx={{ mt: 2 }}>
                                    Verify
                                </Button>
                            </Box>
                        )}

                        {data.id === 0 ? null :
                            <Typography component="p" variant="body2" color="info.main">
                                Fill in fields bellow only if you wish to change your password
                            </Typography>
                        }
                        
                        <Box component={FormGroup} mb={2}>
                            <TextField
                                margin="normal"
                                fullWidth
                                label="New password"
                                name="newPassword"
                                type="password"
                                value={data.newPassword}
                                required={data.id === 0}
                                onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                                    setData({
                                        ...data,
                                        newPassword: e.currentTarget.value
                                    })
                                }
                            />

                            <TextField
                                margin="normal"
                                fullWidth
                                label="New password confirmation"
                                name="newPasswordConfirm"
                                type="password"
                                required={data.id === 0}
                                value={data.newPasswordConfirm}
                                onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                                    setData({
                                        ...data,
                                        newPasswordConfirm: e.currentTarget.value
                                    })
                                }
                            />
                        </Box>

                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mb: 2 }}
                            disabled={saving}
                            color={data.id === 0 ? 'secondary' : 'info'}>
                            {data.id === 0 ? 'Create' : 'Save'}
                        </Button>
                    </Box>
                </Grid>
                <Grid item xs={12}>
                    <Tooltip title="Go home">
                        <Button
                            color={'dark' as 'inherit'}
                            variant="contained"
                            disabled={saving}
                            onClick={() => navigate(UseRoutes[AppRoute.Home].path)}>
                            <Home /> Home
                        </Button>
                    </Tooltip>
                </Grid>
            </Grid>
        </Box>
    );
};

export default UserManage;
