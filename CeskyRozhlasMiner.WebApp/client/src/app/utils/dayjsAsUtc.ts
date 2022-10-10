import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';

dayjs.extend(utc);
const dayjsAsUtc = dayjs.utc;

export default dayjsAsUtc;