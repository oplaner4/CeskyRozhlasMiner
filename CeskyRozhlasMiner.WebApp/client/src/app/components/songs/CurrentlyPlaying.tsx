import React, { useEffect, useState } from 'react';
import { Typography, List, ListItem, CircularProgress, Box } from '@mui/material';
import { ApiClient, ApiException, RozhlasStation, SongDto } from 'app/generated/backend';
import { useSetRecoilState } from 'recoil';
import { getErrorMessage } from 'app/utils/utilities';
import { appAlertsAtom } from 'app/state/atom';
import { timeFormatter } from 'app/utils/grid';
import { AccessTime, MusicNote, Radio } from '@mui/icons-material';

export interface CurrentlyPlayingProps {
    updateIntervalSeconds: number;
}

const CurrentlyPlaying = ({ updateIntervalSeconds }: CurrentlyPlayingProps) => {
    const [playingNow, setPlayingNow] = useState<SongDto[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [lastUpdated, setLastUpdated] = useState<Date | null>(null);

    const setAppAlerts = useSetRecoilState(appAlertsAtom);

    useEffect(() => {
        const fetchPlayingNow = async () => {
            try {
                setLoading(true);
                setPlayingNow(await new ApiClient(process.env.REACT_APP_API_BASE).songs_GetCurrentlyPlayingSongs());
                setLoading(false);
                setLastUpdated(new Date());
            } catch (e) {
                console.log(e);

                setAppAlerts((appAlerts) => [
                    ...appAlerts,
                    {
                        text: getErrorMessage(e as ApiException),
                        severity: 'error'
                    }
                ]);

                setLoading(false);
            }
        };

        const updateInterval = setInterval(fetchPlayingNow, updateIntervalSeconds * 1000);
        fetchPlayingNow();

        return () => {
            clearInterval(updateInterval);
        };
    }, [setAppAlerts, updateIntervalSeconds]);

    return (
        <Box>
            <Typography component="h4" variant="h5">
                Playing now (on air)
            </Typography>

            {loading || lastUpdated === null ? null : (
                <Box ml={2} mt={1}>
                    <Typography variant="body1" component="span" color="dark.main">
                        Last updated: {timeFormatter(lastUpdated)}
                    </Typography>
                </Box>
            )}

            {loading ? (
                <Box ml={2} mb={3}>
                    <Typography component="span" variant="h6" color="primary.main">
                        Updating... <CircularProgress size="1.3rem" />
                    </Typography>
                </Box>
            ) : (
                <Box mb={3}>
                    <List>
                        {playingNow.map((s) => (
                            <ListItem key={s.sourceStation}>
                                <Typography component="h4" variant="h6">
                                    <Typography component="span" variant="inherit" color="primary.main">
                                        <AccessTime fontSize="inherit" /> {timeFormatter(s.playedAt)}
                                    </Typography>
                                    <Box component="span" mx={3}>
                                        <Typography component="span" variant="inherit" color="dark.main">
                                            <Radio fontSize="inherit" /> {RozhlasStation[s.sourceStation]}
                                        </Typography>
                                    </Box>
                                    <Typography component="span" variant="inherit" color="secondary.main">
                                        <MusicNote fontSize="inherit" /> {s.artist} - {s.title}
                                    </Typography>
                                </Typography>
                            </ListItem>
                        ))}
                    </List>
                </Box>
            )}
        </Box>
    );
};

export default CurrentlyPlaying;
