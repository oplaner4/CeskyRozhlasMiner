import { ApiClient, ApiException, RozhlasStation, SongDto } from 'app/generated/backend';
import React, { useEffect, useState } from 'react';
import { GridColDef, GridValueFormatterParams } from '@mui/x-data-grid';
import { Box, Typography } from '@mui/material';
import { useSetRecoilState } from 'recoil';
import { appAlertsAtom } from 'app/state/atom';
import { getErrorMessage } from 'app/utils/utilities';
import AppDataGrid from 'app/components/AppDataGrid';
import { dateTimeValueFormatter } from 'app/utils/grid';
import { useSearchParams } from 'react-router-dom';

const Songs: React.FC = () => {
    const setAppAlerts = useSetRecoilState(appAlertsAtom);
    const [data, setData] = useState<SongDto[]>([]);
    const [loading, setLoading] = useState<boolean>(true);

    const [params] = useSearchParams();
    const playlistId = Number(params.get('id'));

    const columns: GridColDef[] = [
        {
            field: 'artist',
            headerName: 'Artist',
            width: 260,
        },
        {
            field: 'title',
            headerName: 'Title',
            width: 400,
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
            width: 200,
        },
    ];


    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);
                const result = await new ApiClient(process.env.REACT_APP_API_BASE).songs_GetAllSongsForPlaylist(playlistId);
                setData(result.songs);

                if (result.maxLimitExceeded) {
                    setAppAlerts((appAlerts) => [
                        ...appAlerts,
                        {
                            text: <>Showing first{' '}
                            <Typography component="span" fontWeight="bold">{result.maxLimit}</Typography>{' '}
                            songs as the limit which has been exceeded.</>,
                            severity: 'info',
                        }
                    ]);
                }

                setLoading(false);
            } catch (e) {
                console.log(e);

                setAppAlerts((appAlerts) => [
                    ...appAlerts,
                    {
                        text: getErrorMessage(e as ApiException),
                        severity: 'error',
                    }
                ]);

                setLoading(false);
            }
        };

        fetchData();
    }, [setAppAlerts, playlistId]);

    return (
        <Box>
            <Box mb={5}>
                <AppDataGrid
                    rows={data}
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
        </Box>
    );
};

export default Songs;
