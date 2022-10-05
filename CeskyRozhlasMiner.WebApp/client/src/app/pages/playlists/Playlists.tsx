import { ApiClient, ApiException, PlaylistDto, PlaylistSourceStationDto } from 'app/generated/backend';
import React, { useEffect, useState } from 'react';
import { GridColDef } from '@mui/x-data-grid';
import { Box, Button, Typography } from '@mui/material';
import { useSetRecoilState } from 'recoil';
import { appAlertsAtom } from 'app/state/atom';
import { getErrorMessage } from 'app/utils/utilities';
import AppDataGrid from 'app/components/AppDataGrid';
import { dateTimeValueFormatter } from 'app/utils/grid';
import { Add } from '@mui/icons-material';
import AppModal from 'app/components/AppModal';
import PlaylistManage from './PlaylistManage';

const columns: GridColDef[] = [
  {
    field: 'id',
    headerName: 'ID',
    type: 'number',
    width: 110
  },
  {
    field: 'name',
    headerName: 'Name',
    width: 300,
    editable: true,
  },
  {
    field: 'createdDate',
    headerName: 'Created',
    type: 'number',
    width: 200,
    valueFormatter: dateTimeValueFormatter,
  },
  {
    field: 'updatedDate',
    headerName: 'Updated',
    type: 'number',
    width: 200,
    valueFormatter: dateTimeValueFormatter,
  },
];

const Playlists: React.FC = () => {
  const setAppAlerts = useSetRecoilState(appAlertsAtom);
    const [data, setData] = useState<PlaylistDto[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [creating, setCreating] = useState<boolean>(false);
  
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
                      severity: 'error',
                    }
                ]);

                setLoading(false);
            }
        };

        fetchData();
    }, [setAppAlerts]);

    return (
      <Box>
          <Box component={Typography} variant="body1" textAlign={{ md: "right" }} mb={3}>
            <Button variant="contained" color="success" onClick={() => setCreating(true)}>
                <Add /> Add
            </Button>
            <AppModal
              title="Add playlist"
              ariaPrefix="playlist-create"
              content={
                <PlaylistManage
                  source={{
                    name: "",
                    sourceStations: [] as PlaylistSourceStationDto[],
                    from: new Date(),
                    to: new Date(),
                    ownerId: 0,
                    id: 0,
                    createdDate: new Date(),
                    updatedDate: new Date(),
                  }}
                  setSource={(created: PlaylistDto|null) => {
                    if (created !== null) {
                      setData([
                        ...data,
                        created,
                      ]);
                    }

                    setCreating(false);
                  }}
                />
              }
              opened={creating}
              onClose={setCreating}
            />
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
                              sort: 'desc',
                          },
                      ],
                  },
                }}
              />
          </Box>
      </Box>

    );
};

export default Playlists;
