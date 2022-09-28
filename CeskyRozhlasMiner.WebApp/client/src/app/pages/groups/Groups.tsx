import { ApiClient, IGroupDto } from 'app/generated/backend';
import React, { useEffect, useState } from 'react';
import { DataGrid, GridColDef, GridValueGetterParams } from '@mui/x-data-grid';
import CircularProgress from '@mui/material/CircularProgress';
import { Box } from '@mui/material';
import { useSetRecoilState } from 'recoil';
import { appAlertsAtom } from 'app/state/atom';

const columns: GridColDef[] = [
  { field: 'id', headerName: 'ID', width: 90 },
  {
    field: 'name',
    headerName: 'Name',
    width: 150,
    editable: true,
  },
  {
    field: 'isActive',
    headerName: 'Active',
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

const Groups: React.FC = () => {
  const setAppAlerts = useSetRecoilState(appAlertsAtom);

    const [data, setData] = useState({
        groups: [] as IGroupDto[],
        isFetching: false
    });
    
    useEffect(() => {
        const fetchData = async () => {
            try {
                setData({ groups: data.groups, isFetching: true });
                const result = await new ApiClient(process.env.REACT_APP_API_BASE).groups_GetAllGroups();
                setData({ groups: result, isFetching: false });
            } catch (e) {
                console.log(e);
                setAppAlerts((appAlerts) => [
                  ...appAlerts,
                  {
                    text: 'Unable to load',
                    severity: 'error',
                  }
                ])
                setData({ groups: data.groups, isFetching: false });
            }
        };

        fetchData();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    return (
        <>
            <Box component="div" style={{ height: 400, width: '100%' }}>
              <DataGrid
                rows={data.groups}
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

export default Groups;
