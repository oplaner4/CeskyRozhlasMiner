import { Alert, Box, Button, FormGroup, TextField, FormControlLabel, Checkbox, Typography } from '@mui/material';
import { DesktopDatePicker } from '@mui/x-date-pickers';
import {
    ApiClient,
    ApiException,
    IPlaylistDto,
    PlaylistDto,
    PlaylistSourceStationDto,
    RozhlasStation
} from 'app/generated/backend';
import { appAlertsAtom } from 'app/state/atom';
import dayjsAsUtc from 'app/utils/dayjsAsUtc';
import { useDateFormat } from 'app/utils/localization';
import { Dayjs } from 'dayjs';

import { useState } from 'react';
import { useSetRecoilState } from 'recoil';
import FormattedErrorMessage from '../FormattedErrorMessage';


export interface PlaylistManageProps {
    source: IPlaylistDto;
    setSource: (updated: PlaylistDto | null) => void;
}

const PlaylistManage: React.FC<PlaylistManageProps> = ({ source, setSource }: PlaylistManageProps) => {
    const setAppAlerts = useSetRecoilState(appAlertsAtom);

    const [saving, setSaving] = useState<boolean>(false);

    const [data, setData] = useState<IPlaylistDto>({
        ...source
    });

    const [messageContent, setMessageContent] = useState<React.ReactElement | null>(null);

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        const saveData = async () => {
            try {
                setSaving(true);
                let saveData = new PlaylistDto();
                saveData.init(data);

                saveData.from = dayjsAsUtc(data.from).local().hour(0).minute(0).second(0).millisecond(0).utc() as unknown as Date;
                saveData.to = dayjsAsUtc(data.to).local().hour(23).minute(59).second(59).millisecond(999).utc() as unknown as Date;

                if (source.id === 0) {
                    saveData = await new ApiClient(process.env.REACT_APP_API_BASE).playlists_CreatePlaylist(saveData);
                } else {
                    saveData = await new ApiClient(process.env.REACT_APP_API_BASE).playlists_UpdatePlaylist(saveData);
                }

                setSource(saveData);
                setAppAlerts((appAlerts) => [
                    ...appAlerts,
                    {
                        severity: 'success',
                        text: `Playlist successfully ${source.id === 0 ? 'created' : 'edited'}.`
                    }
                ]);
                setSaving(false);
            } catch (e) {
                console.log(e);
                setMessageContent(<FormattedErrorMessage exception={e as ApiException} />);
                setSaving(false);
            }
        };

        saveData();
    };

    return (
        <Box>
            {messageContent === null ? null : (
                <Box component={Alert} mb={3} severity="error">
                    {messageContent}
                </Box>
            )}
            <Box component="form" onSubmit={handleSubmit} noValidate mt={1}>
                <TextField
                    margin="normal"
                    fullWidth
                    label="Id"
                    name="id"
                    value={data.id}
                    InputProps={{
                        readOnly: true,
                    }}
                    variant="filled"
                />

                <TextField
                    margin="normal"
                    required
                    fullWidth
                    label="Name"
                    name="name"
                    autoFocus
                    value={data.name}
                    onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                        setData({
                            ...data,
                            name: e.currentTarget.value
                        })
                    }
                />

                <DesktopDatePicker
                    label="From"
                    inputFormat={useDateFormat}
                    value={dayjsAsUtc(data.from).local()}
                    onChange={(newValue: Dayjs | null) => {
                        if (newValue === null) {
                            return;
                        }

                        setData({
                            ...data,
                            from: newValue.utc() as unknown as Date,
                        });
                    }
                        
                    }
                    renderInput={(params) => <TextField {...params} margin="normal" required fullWidth name="from" />}
                />

                <DesktopDatePicker
                    label="To"
                    inputFormat={useDateFormat}
                    value={dayjsAsUtc(data.to).local()}
                    onChange={(newValue: Dayjs | null) => {
                        if (newValue === null) {
                            return;
                        }

                        setData({
                            ...data,
                            to: newValue.utc() as unknown as Date,
                        });
                    }}
                    renderInput={(params) => <TextField {...params} margin="normal" required fullWidth name="to" />}
                />

                <Box mt={1}>
                    <Typography variant="h6" component="h3">
                        Source stations
                    </Typography>
                </Box>

                <Box sx={{ overflow: 'auto', maxHeight: 200 }} ml={{ md: 2 }}>
                    {Object.keys(RozhlasStation)
                        .filter((s) => !isNaN(Number(s)))
                        .map((s) => {
                            const inx = Number(s);

                            return (
                                <FormGroup key={s}>
                                    <FormControlLabel
                                        control={
                                            <Checkbox
                                                checked={
                                                    data.sourceStations.filter(
                                                        (s) => s.station === (inx as unknown as RozhlasStation)
                                                    ).length > 0
                                                }
                                                onChange={(e: React.ChangeEvent<HTMLInputElement>) => {
                                                    const dto = new PlaylistSourceStationDto();
                                                    dto.station = inx as unknown as RozhlasStation;

                                                    setData({
                                                        ...data,
                                                        sourceStations: e.currentTarget.checked
                                                            ? [...data.sourceStations, dto]
                                                            : data.sourceStations.filter((s) => s.station !== dto.station)
                                                    });
                                                }}
                                            />
                                        }
                                        label={RozhlasStation[inx]}
                                    />
                                </FormGroup>
                            );
                        })}
                </Box>

                <Button type="submit" fullWidth variant="contained" sx={{ mt: 3, mb: 2 }} disabled={saving}>
                    Save
                </Button>
            </Box>
        </Box>
    );
};

export default PlaylistManage;
