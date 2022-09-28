import { atom } from 'recoil';
import { IUserDto } from 'app/generated/backend';
import { IAppAlert } from 'app/models/IAppAlert';

export const userAtom = atom<IUserDto|null>({
  key: 'user',
  default: null,
});

export const appAlertsAtom = atom<IAppAlert[]>({
  key: 'appAlerts',
  default: [],
});