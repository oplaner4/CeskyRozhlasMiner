import { List, ListItem, Box } from "@mui/material";
import { ApiException, ValidationProblemDetails } from "app/generated/backend";

export interface FormattedErrorMessageProps {
    exception: ApiException | ValidationProblemDetails;
}

const FormattedErrorMessage = ({exception}: FormattedErrorMessageProps): React.ReactElement => {
    if (exception instanceof ValidationProblemDetails) {
        return <Box component={List} dense p={0}>
            {Object.keys(exception.errors).map(er => {
                return <ListItem key={er}>
                    {exception.errors[er].join(' ')}
                </ListItem>
            })}
        </Box>;
    }

    if (!ApiException.isApiException(exception)) {
        return <>{(exception as ApiException).message}</>;
    }

    const response = JSON.parse(exception.response);

    if (response !== null) {
        
        return <>{response["Message"]}</>;
    }

    return <>{(exception as ApiException).message}</>;
}

export default FormattedErrorMessage;