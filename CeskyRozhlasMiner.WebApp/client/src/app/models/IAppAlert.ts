import { AlertColor } from '@mui/material';

export interface IAppAlert {
    text: string | React.ReactElement;
    severity: AlertColor;
}