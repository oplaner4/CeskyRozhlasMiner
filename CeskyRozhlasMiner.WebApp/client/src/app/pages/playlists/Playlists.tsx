import { ApiClient, ApiException, IPlaylistDto } from 'app/generated/backend';
import React, { useEffect, useState } from 'react';
import { DataGrid, GridColDef, GridValueGetterParams } from '@mui/x-data-grid';
import CircularProgress from '@mui/material/CircularProgress';
import { Box } from '@mui/material';
import { useSetRecoilState } from 'recoil';
import { appAlertsAtom } from 'app/state/atom';
import { getErrorMessage } from 'app/utils/utilities';

const columns: GridColDef[] = [
  { field: 'id', headerName: 'ID', width: 90 },
  {
    field: 'name',
    headerName: 'Name',
    width: 150,
    editable: true,
  },
  {
    field: 'createdDate',
    headerName: 'Created date',
    type: 'number',
    width: 110,
    editable: false,
    valueGetter: (params: GridValueGetterParams) => `${params.value.toLocaleString()}`,
  },
  {
    field: 'updatedDate',
    headerName: 'Updated Date',
    sortable: false,
    width: 160,
    valueGetter: (params: GridValueGetterParams) => `${params.value.toLocaleString()}`,
  },
];

const Playlists: React.FC = () => {
  const setAppAlerts = useSetRecoilState(appAlertsAtom);

    const [data, setData] = useState({
        playlists: [] as IPlaylistDto[],
        isFetching: false
    });
    
    useEffect(() => {
        const fetchData = async () => {
            try {
                setData({ playlists: data.playlists, isFetching: true });
                const result = await new ApiClient(process.env.REACT_APP_API_BASE).playlists_GetAllUserPlaylists();
                setData({ playlists: result, isFetching: false });
            } catch (e) {
                console.log(e);
            
                setAppAlerts((appAlerts) => [
                    ...appAlerts,
                    {
                      text: getErrorMessage(e as ApiException),
                      severity: 'error',
                    }
                ]);
                setData({ playlists: data.playlists, isFetching: false });
            }
        };

        fetchData();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    return (
        <>
            <Box component="div" style={{ height: 400, width: '100%' }}>
              <DataGrid
                rows={data.playlists}
                columns={columns}
                pageSize={5}
                rowsPerPageOptions={[5]}
                checkboxSelection
                disableSelectionOnClick
              />
            </Box>

            {data.isFetching && <CircularProgress />}
        </>
    );
};

export default Playlists;
