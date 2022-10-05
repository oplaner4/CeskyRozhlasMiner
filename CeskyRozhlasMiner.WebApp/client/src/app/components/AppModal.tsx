import * as React from 'react';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import Modal from '@mui/material/Modal';

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
    content: React.ReactElement;
    opened: boolean;
    onClose: (opened: boolean) => void;
    ariaPrefix: string;
}

const AppModal: React.FC<AppModalProps> = ({title, content, opened, onClose, ariaPrefix}: AppModalProps) => {
    const ariaTitle = `${ariaPrefix}-title`;
    const ariaDescription = `${ariaPrefix}-description`;

    return (
        <Modal
          open={opened}
          onClose={() => onClose(false)}
          aria-labelledby={ariaTitle}
          aria-describedby={ariaDescription}
        >
          <Box sx={style}>
            <Typography id={ariaTitle} variant="h5" component="h2">
              {title}
            </Typography>
            <Typography component={Box} id={ariaDescription} my={2}>
              {content}
            </Typography>
            <Box display="flex" justifyContent={{ xs: "center", md: "flex-end" }}>
              <Button onClick={() => onClose(false)} variant="contained" color="error">Close</Button>
            </Box>
          </Box>
        </Modal>
    );
}

export default AppModal;