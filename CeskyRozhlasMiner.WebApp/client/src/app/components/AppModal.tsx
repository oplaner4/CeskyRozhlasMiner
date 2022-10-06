import * as React from 'react';
import Box from '@mui/material/Box';
import Button, { ButtonProps } from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import Modal from '@mui/material/Modal';
import { ButtonGroup } from '@mui/material';

const style = {
  position: 'absolute' as 'absolute',
  top: '50%',
  left: '50%',
  transform: 'translate(-50%, -50%)',
  width: 400,
  bgcolor: 'background.paper',
  border: '2px solid #000',
  boxShadow: 24,
  p: 4,
  maxHeight: '100%',
  overflow: 'auto',
};

export interface AppModalProps {
    title: string;
    content: React.ReactElement | null;
    onClose: (content: React.ReactElement | null) => void;
    ariaPrefix: string;
    onActionBtnClick?: () => void;
    actionBtnValue?: React.ReactNode | string;
    actionBtnProps?: ButtonProps;
}

const AppModal: React.FC<AppModalProps> = ({title, content, onClose, ariaPrefix,
      onActionBtnClick, actionBtnProps, actionBtnValue}: AppModalProps) => {
    const ariaTitle = `${ariaPrefix}-title`;
    const ariaDescription = `${ariaPrefix}-description`;

    return (
        <Modal
          onClose={() => onClose(null)}
          aria-labelledby={ariaTitle}
          aria-describedby={ariaDescription}
          open={content !== null}
        >
          <Box sx={style}>
            <Typography id={ariaTitle} variant="h5" component="h2">
              {title}
            </Typography>
            <Typography component={Box} id={ariaDescription} my={2}>
              {content}
            </Typography>
            <Box display="flex" justifyContent={{ xs: "center", md: "flex-end" }}>
              <ButtonGroup size="small">
                {actionBtnValue ?
                  <Box mr={2}>
                    <Button onClick={onActionBtnClick} variant="contained" color="primary" {...actionBtnProps}>
                      {actionBtnValue}
                    </Button>
                  </Box>
                : null}
                <Button onClick={() => onClose(null)} variant="contained" color="error">Close</Button>
              </ButtonGroup>
            </Box>
          </Box>
        </Modal>
    );
}

export default AppModal;