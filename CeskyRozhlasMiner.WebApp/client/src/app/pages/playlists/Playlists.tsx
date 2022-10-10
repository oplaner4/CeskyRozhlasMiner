import { ApiClient, ApiException, PlaylistDto } from 'app/generated/backend';
import React, { useEffect, useState } from 'react';
import { GridColDef, GridRenderCellParams } from '@mui/x-data-grid';
import { Box, Button, ButtonGroup, Tooltip, Typography } from '@mui/material';
import { useSetRecoilState } from 'recoil';
import { appAlertsAtom } from 'app/state/atom';
import { getErrorMessage } from 'app/utils/utilities';
import AppDataGrid from 'app/components/AppDataGrid';
import { dateFormatter, dateTimeValueFormatter } from 'app/utils/grid';
import { Add, Delete, Edit, MusicNote } from '@mui/icons-material';
import AppModal from 'app/components/AppModal';
import PlaylistManage from '../../components/playlist/PlaylistManage';
import { useNavigate } from 'react-router-dom';
import { AppRoute, UseRoutes } from 'app/components/AppRoutes';
import dayjsAsUtc from 'app/utils/dayjsAsUtc';

const Playlists: React.FC = () => {
    const setAppAlerts = useSetRecoilState(appAlertsAtom);
    const [data, setData] = useState<PlaylistDto[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [shouldRefetch, setShouldRefetch] = useState<boolean>(false);
    const navigate = useNavigate();

    const now = dayjsAsUtc();
    const createManagePlaylist = (
        <PlaylistManage
            source={{
                name: '',
                sourceStations: [],
                from: now.hour(0).minute(0).second(0).millisecond(0) as unknown as Date,
                to: now.hour(23).minute(59).second(59).millisecond(59) as unknown as Date,
                ownerId: 0,
                id: 0,
                createdDate: now as unknown as Date,
                updatedDate: now as unknown as Date,
            }}
            setSource={(created: PlaylistDto | null) => {
                if (created !== null) {
                    setData([...data, created]);
                }

                setCreatePlaylistModalContent(null);
            }}
        />
    );

    const [createPlaylistModalContent, setCreatePlaylistModalContent] = useState<React.ReactElement | null>(null);
    const [deletePlaylistModalContent, setDeletePlaylistModalContent] = useState<React.ReactElement | null>(null);
    const [editPlaylistModalContent, setEditPlaylistModalContent] = useState<React.ReactElement | null>(null);
    const [deletePlaylistHandler, setDeletePlaylistHandler] = useState<() => Promise<void>>();

    const columns: GridColDef[] = [
        {
            field: 'name',
            headerName: 'Name',
            width: 260,
            editable: true
        },
        {
            field: 'from',
            headerName: 'Date range',
            type: 'number',
            width: 200,
            renderCell: (params: GridRenderCellParams<Date, PlaylistDto>) => {
                return (
                    <>
                        {dateFormatter(params.row.from)} - {dateFormatter(params.row.to)}
                    </>
                );
            }
        },
        {
            field: 'id',
            headerName: 'Action',
            sortable: false,
            width: 160,
            renderCell: (params: GridRenderCellParams<number, PlaylistDto>) => {
                return (
                    <ButtonGroup size="small" variant="contained">
                        <Tooltip title="Delete playlist">
                            <Button
                                color="error"
                                onClick={() => {
                                    setDeletePlaylistModalContent(
                                        <Typography variant="subtitle1">
                                            Do you really wish to delete{' '}
                                            <Typography variant="inherit" component="span" fontWeight="bold">
                                                {params.row.name}
                                            </Typography>
                                            ?
                                        </Typography>
                                    );
                                    setDeletePlaylistHandler(() => () => handlePlaylistDelete(params.row));
                                }}>
                                <Delete />
                            </Button>
                        </Tooltip>
                        <Tooltip title="Edit playlist">
                            <Button
                                color="primary"
                                onClick={() => {
                                    setEditPlaylistModalContent(
                                        <PlaylistManage
                                            source={params.row}
                                            setSource={(edited: PlaylistDto | null) => {
                                                if (edited !== null) {
                                                    setShouldRefetch(!shouldRefetch);
                                                }

                                                setEditPlaylistModalContent(null);
                                            }}
                                        />
                                    );
                                }}>
                                <Edit />
                            </Button>
                        </Tooltip>
                        <Tooltip title="View songs">
                            <Button
                                color={'dark' as 'inherit'}
                                onClick={() => {
                                    navigate(`${UseRoutes[AppRoute.Songs].path}?id=${params.id}`);
                                }}>
                                <MusicNote />
                            </Button>
                        </Tooltip>
                    </ButtonGroup>
                );
            }
        },
        {
            field: 'createdDate',
            headerName: 'Created',
            type: 'number',
            width: 300,
            valueFormatter: dateTimeValueFormatter
        },
        {
            field: 'updatedDate',
            headerName: 'Updated',
            type: 'number',
            width: 300,
            valueFormatter: dateTimeValueFormatter
        }
    ];

    const handlePlaylistDelete = async (dto: PlaylistDto): Promise<void> => {
        try {
            await new ApiClient(process.env.REACT_APP_API_BASE).playlists_DeletePlaylist(dto.id);
            setAppAlerts((appAlerts) => [
                ...appAlerts,
                {
                    text: (
                        <>
                            Playlist{' '}
                            <Typography component="span" fontWeight="bold">
                                dto.name
                            </Typography>{' '}
                            successfully deleted.
                        </>
                    ),
                    severity: 'success'
                }
            ]);
            setShouldRefetch(!shouldRefetch);
        } catch (e) {
            console.log(e);
            setAppAlerts((appAlerts) => [
                ...appAlerts,
                {
                    text: getErrorMessage(e as ApiException),
                    severity: 'error'
                }
            ]);
        }

        setDeletePlaylistModalContent(null);
    };

    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);
                const result = await new ApiClient(process.env.REACT_APP_API_BASE).playlists_GetAllUserPlaylists();
                setData(result);
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
    }, [setAppAlerts, shouldRefetch]);

    return (
        <Box>
            <Box component={Typography} variant="body1" textAlign={{ md: 'right' }} mb={3}>
                <Button variant="contained" color="success" onClick={() => setCreatePlaylistModalContent(createManagePlaylist)}>
                    <Add /> Add
                </Button>
            </Box>
            <Box mb={5}>
                <AppDataGrid
                    rows={data}
                    columns={columns}
                    loading={loading}
                    initialState={{
                        sorting: {
                            sortModel: [
                                {
                                    field: 'createdDate',
                                    sort: 'desc'
                                }
                            ]
                        }
                    }}
                />
            </Box>
            <AppModal
                title="Delete playlist"
                ariaPrefix="playlist-delete"
                content={deletePlaylistModalContent}
                onClose={setDeletePlaylistModalContent}
                actionBtnValue="Delete"
                onActionBtnClick={deletePlaylistHandler}
                actionBtnProps={{
                    variant: 'outlined'
                }}
            />
            <AppModal
                title="Add playlist"
                ariaPrefix="playlist-create"
                content={createPlaylistModalContent}
                onClose={setCreatePlaylistModalContent}
            />
            <AppModal
                title="Edit playlist"
                ariaPrefix="playlist-edit"
                content={editPlaylistModalContent}
                onClose={setEditPlaylistModalContent}
            />
        </Box>
    );
};

export default Playlists;
