import { ApiClient, ApiException, GetSongsForPlaylistDto, PlaylistDto, RozhlasStation } from 'app/generated/backend';
import React, { useEffect, useState } from 'react';
import { GridColDef, GridValueFormatterParams } from '@mui/x-data-grid';
import { Box, Button, ButtonGroup, Chip, Grid, Tooltip, Typography } from '@mui/material';
import { useSetRecoilState } from 'recoil';
import { appAlertsAtom } from 'app/state/atom';
import { getErrorMessage } from 'app/utils/utilities';
import AppDataGrid from 'app/components/AppDataGrid';
import { dateTimeFormatter, dateTimeValueFormatter } from 'app/utils/grid';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { AppRoute, UseRoutes } from 'app/components/AppRoutes';
import { ArrowBack, Info, Refresh } from '@mui/icons-material';
import dayjsAsUtc from 'app/utils/dayjsAsUtc';

const Songs: React.FC = () => {
    const setAppAlerts = useSetRecoilState(appAlertsAtom);
    const [data, setData] = useState<GetSongsForPlaylistDto | null>(null);
    const [playlist, setPlaylist] = useState<PlaylistDto | null>(null);

    const [loading, setLoading] = useState<boolean>(true);
    const [fetchAgain, setFetchAgain] = useState<boolean>(false);

    const [params] = useSearchParams();
    const playlistId = Number(params.get('id'));

    const navigate = useNavigate();

    const columns: GridColDef[] = [
        {
            field: 'artist',
            headerName: 'Artist',
            width: 260
        },
        {
            field: 'title',
            headerName: 'Title',
            width: 400
        },
        {
            field: 'playedAt',
            headerName: 'Played at',
            type: 'number',
            width: 300,
            valueFormatter: dateTimeValueFormatter
        },
        {
            field: 'sourceStation',
            headerName: 'Station',
            valueFormatter: (params: GridValueFormatterParams<number>): string => {
                return RozhlasStation[params.value];
            },
            width: 200
        }
    ];

    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);
                const result = await new ApiClient(process.env.REACT_APP_API_BASE).songs_GetAllSongsForPlaylist(playlistId);
                setData(result);

                if (result.maxLimitExceeded) {
                    setAppAlerts((appAlerts) => [
                        ...appAlerts,
                        {
                            text: (
                                <>
                                    Showing first{' '}
                                    <Typography component="span" fontWeight="bold">
                                        {result.maxLimit}
                                    </Typography>{' '}
                                    of{' '}
                                    <Typography component="span" fontWeight="bold">
                                        {result.totalCount}
                                    </Typography>{' '}
                                    songs as the limit which has been exceeded.
                                </>
                            ),
                            severity: 'info'
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
    }, [setAppAlerts, playlistId, fetchAgain]);

    return (
        <Box>
            {playlist === null ? null : (
                <Grid component={Box} mb={3} container spacing={2}>
                    <Grid item xs={12} md={6}>
                        <Box mb={2}>
                            <Box mb={1}>
                                <Typography variant="h6" component="h5">
                                    Found{' '}
                                    <Typography component="span" variant="inherit" color="secondary.main">
                                        {data === null ? null : data.totalCount}
                                    </Typography>{' '}
                                    on playlist{' '}
                                    <Typography component="span" variant="inherit" color="secondary.main">
                                        {playlist.name}
                                    </Typography>
                                </Typography>
                            </Box>

                            <Typography variant="body1" component="p" color="primary.main">
                                {dateTimeFormatter(playlist.from)} - {dateTimeFormatter(playlist.to)}
                            </Typography>
                        </Box>
                    </Grid>
                    <Grid item xs={12} md={5} sx={{ display: 'flex' }} alignItems="center">
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
                    </Grid>
                    <Grid
                        item
                        xs={12}
                        md={1}
                        sx={{ display: 'flex' }}
                        alignItems="center"
                        justifyContent={{ xs: 'center', md: 'flex-end' }}>
                        {dayjsAsUtc(playlist.to) >= dayjsAsUtc().startOf('date') ? (
                            <Tooltip title="Refresh">
                                <Button
                                    variant="contained"
                                    color={'success'}
                                    onClick={() => {
                                        setFetchAgain(!fetchAgain);
                                    }}>
                                    <Refresh />
                                </Button>
                            </Tooltip>
                        ) : null}
                    </Grid>
                </Grid>
            )}

            <Box mb={5}>
                <AppDataGrid
                    rows={data === null ? [] : data.songs}
                    columns={columns}
                    loading={loading}
                    initialState={{
                        sorting: {
                            sortModel: [
                                {
                                    field: 'playedAt',
                                    sort: 'desc'
                                }
                            ]
                        }
                    }}
                />
            </Box>

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
                <Tooltip title="See information">
                    <Button
                        variant="contained"
                        color={'info'}
                        onClick={() => {
                            navigate(`${UseRoutes[AppRoute.PlaylistInfo].path}?id=${playlistId}`);
                        }}>
                        <Info /> Information
                    </Button>
                </Tooltip>
            </ButtonGroup>
        </Box>
    );
};

export default Songs;
