import { ApiClient, ApiException, GetStatisticsForPlaylistDto, PlaylistDto, RozhlasStation } from 'app/generated/backend';
import React, { useEffect, useState } from 'react';
import { Box, Button, ButtonGroup, Chip, CircularProgress, Grid, List, ListItem, Tooltip, Typography } from '@mui/material';
import { useSetRecoilState } from 'recoil';
import { appAlertsAtom } from 'app/state/atom';
import { getErrorMessage } from 'app/utils/utilities';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { dateFormatter, dateTimeFormatter } from 'app/utils/grid';
import { ArrowBack, MusicNote } from '@mui/icons-material';
import { AppRoute, UseRoutes } from 'app/components/AppRoutes';

const PlaylistInfo: React.FC = () => {
    const setAppAlerts = useSetRecoilState(appAlertsAtom);
    const [data, setData] = useState<GetStatisticsForPlaylistDto | null>(null);
    const [playlist, setPlaylist] = useState<PlaylistDto | null>(null);
    const [loading, setLoading] = useState<boolean>(true);

    const [params] = useSearchParams();
    const playlistId = Number(params.get('id'));

    const navigate = useNavigate();

    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);
                const result = await new ApiClient(process.env.REACT_APP_API_BASE).statistics_GetStatisticsForPlaylist(
                    playlistId
                );
                setData(result);

                if (result.noSourceData) {
                    setAppAlerts((appAlerts) => [
                        ...appAlerts,
                        {
                            text: 'No source data available.',
                            severity: 'error'
                        }
                    ]);
                }

                setPlaylist(await new ApiClient(process.env.REACT_APP_API_BASE).playlists_GetPlaylist(playlistId));

                setLoading(false);
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

        fetchData();
    }, [setAppAlerts, playlistId]);

    return (
        <Box component="div" mb={3}>
            {loading ? (
                <Box mb={4}>
                    <CircularProgress />
                </Box>
            ) : null}

            {playlist === null ? null : (
                <Box mb={4}>
                    <Box mb={2}>
                        <Typography variant="h6" component="h5">
                            Statistics for{' '}
                            <Typography component="span" variant="inherit" color="secondary.main">
                                {playlist.name}{' '}
                            </Typography>
                        </Typography>
                        <Typography variant="body1" component="p" color="primary.main">
                            {dateTimeFormatter(playlist.from)} - {dateTimeFormatter(playlist.to)}
                        </Typography>
                    </Box>

                    {playlist.sourceStations.map((s) => (
                        <Chip
                            color={'dark' as 'default'}
                            component={Box}
                            mr={1}
                            key={s.station}
                            label={RozhlasStation[s.station]}
                            variant="filled"
                        />
                    ))}
                </Box>
            )}

            {data !== null && !data.noSourceData ? (
                <Grid component={Box} mb={4} container spacing={3} justifyContent={{ xs: 'center', md: 'flex-start' }}>
                    <Grid item xs={12} textAlign={{ xs: 'center', md: 'left' }}>
                        <Typography variant="h4" component="h3" color="primary.main">
                            Records
                        </Typography>
                    </Grid>
                    <Grid item md={6} lg={4}>
                        <Typography variant="h5" component="h4" color="secondary.main">
                            {data.mostFrequentArtist.value}
                        </Typography>
                        <Typography variant="h4" component="span" align="center" color="warning.main">
                            {data.mostFrequentArtist.count}x
                        </Typography>
                        <Typography variant="h6" component="h3">
                            (Most frequent artist)
                        </Typography>
                    </Grid>
                    <Grid item md={6} lg={4}>
                        <Typography variant="h5" component="h4" color="secondary.main">
                            {data.mostPlayedSong.value}
                        </Typography>
                        <Typography variant="h4" component="span" align="center" color="warning.main">
                            {data.mostPlayedSong.count}x
                        </Typography>
                        <Typography variant="h6" component="h3">
                            (Most played song)
                        </Typography>
                    </Grid>
                    <Grid item md={12} lg={12}>
                        <Typography variant="h4" component="h3" color="primary.main">
                            Leader board
                        </Typography>
                        <Box display="flex" justifyContent={{ xs: 'center', md: 'flex-start' }}>
                            <List>
                                {data.leaderBoard.map((s, i) => (
                                    <ListItem key={s.value}>
                                        <Grid container spacing={2}>
                                            <Grid item xs={2} sm={1} md={1}>
                                                <Typography variant="h5" component="span" align="center">
                                                    {i + 1}.
                                                </Typography>
                                            </Grid>
                                            <Grid item xs={8} sm={10} md={10}>
                                                <Typography variant="h5" component="h4" color="secondary.main">
                                                    {s.value}
                                                </Typography>
                                            </Grid>
                                            <Grid item xs={2} sm={1} md={1}>
                                                <Typography variant="h5" component="span" align="center" color="warning.main">
                                                    {s.count}x
                                                </Typography>
                                            </Grid>
                                        </Grid>
                                    </ListItem>
                                ))}
                            </List>
                        </Box>
                    </Grid>
                </Grid>
            ) : null}

            <ButtonGroup>
                <Tooltip title="Back to playlists">
                    <Button
                        variant="contained"
                        color={'dark' as 'inherit'}
                        onClick={() => {
                            navigate(UseRoutes[AppRoute.Playlists].path);
                        }}>
                        <ArrowBack />
                        Back
                    </Button>
                </Tooltip>
                <Tooltip title="View songs">
                    <Button
                        variant="contained"
                        color={'info'}
                        onClick={() => {
                            navigate(`${UseRoutes[AppRoute.Songs].path}?id=${playlistId}`);
                        }}>
                        <MusicNote /> Songs
                    </Button>
                </Tooltip>
            </ButtonGroup>
        </Box>
    );
};

export default PlaylistInfo;
