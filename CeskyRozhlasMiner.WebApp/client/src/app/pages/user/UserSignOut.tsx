import { Typography } from '@mui/material';
import { ApiClient, ApiException } from 'app/generated/backend';
import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

import { appAlertsAtom, userAtom } from 'app/state/atom';
import { useSetRecoilState } from 'recoil';
import { getErrorMessage } from 'app/utils/utilities';
import { AppRoute, UseRoutes } from 'app/components/AppRoutes';

const UserSignOut: React.FC = () => {
    const navigate = useNavigate();
    const setAppAlerts = useSetRecoilState(appAlertsAtom);
    const setUser = useSetRecoilState(userAtom);

    useEffect(() => {
        try {
            const signOut = async () => {
                await new ApiClient(process.env.REACT_APP_API_BASE).users_SignOutUser();
                setUser(null);
                navigate(UseRoutes[AppRoute.Default].path);
            };
            
            signOut();
        }
        catch (e) {
            console.log(e);
            
            setAppAlerts((appAlerts) => [
                ...appAlerts,
                {
                  text: getErrorMessage(e as ApiException),
                  severity: 'error',
                }
              ]);
        }
        
    // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    return <Typography variant="body1">Signing out...</Typography>;
};

export default UserSignOut;