import { Home, Person } from '@mui/icons-material';
import { Box, Button, Typography, ButtonGroup } from '@mui/material';
import { ApiClient, ApiException, TokenDto } from 'app/generated/backend';
import { appAlertsAtom, userAtom } from 'app/state/atom';
import { getErrorMessage } from 'app/utils/utilities';
import { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useSetRecoilState } from 'recoil';
import { AppRoute, UseRoutes } from '../AppRoutes';

export const TokenVerifyUser: React.FC = () => {
    const [params] = useSearchParams();
    const setAppAlerts = useSetRecoilState(appAlertsAtom);
    const setUser = useSetRecoilState(userAtom);
    const payload = params.get('payload');
    const [verified, setVerified] = useState<boolean>(false);

    const navigate = useNavigate();

    useEffect(() => {
        const verifyUser = async () => {
            try {
                const sendData = new TokenDto();
                sendData.value = payload ?? '';

                setUser(await new ApiClient(process.env.REACT_APP_API_BASE).tokens_VerifyUser(sendData));
                setAppAlerts((appAlerts) => [
                    ...appAlerts,
                    {
                        severity: 'success',
                        text: <>Successfully verified</>
                    }
                ]);
                setVerified(true);
            } catch (e) {
                console.log(e);
                setAppAlerts((appAlerts) => [
                    ...appAlerts,
                    {
                        severity: 'error',
                        text: getErrorMessage(e as ApiException)
                    }
                ]);
            }
        };

        verifyUser();
    }, [payload, setAppAlerts, setUser]);

    return (
        <Box>
            {verified ? (
                <ButtonGroup>
                    <Button
                        variant="contained"
                        color={'dark' as 'inherit'}
                        onClick={() => navigate(UseRoutes[AppRoute.Home].path)}>
                        <Home /> Home
                    </Button>
                    <Button variant="contained" color={'info'} onClick={() => navigate(UseRoutes[AppRoute.UserSettings].path)}>
                        <Person /> User settings
                    </Button>
                </ButtonGroup>
            ) : (
                <Typography component="p" variant="body1" color="warning.main">
                    Verifying...
                </Typography>
            )}
        </Box>
    );
};

export default TokenVerifyUser;
