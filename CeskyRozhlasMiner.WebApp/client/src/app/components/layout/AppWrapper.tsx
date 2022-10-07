import { Box, BoxProps, Grid } from "@mui/material";

const AppWrapper: React.FC<BoxProps> = (props: BoxProps) => {
    return <Box component={Grid} container justifyContent="center" px={4} {...props}>
        <Grid item md={10} lg={8} xs={12}>
            {props.children}
        </Grid>
    </Box>
}

export default AppWrapper;