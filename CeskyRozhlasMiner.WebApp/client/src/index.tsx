import MsalComponent from 'MsalComponent';

import ReactDOM from 'react-dom';
import React from 'react';

import { RecoilRoot } from 'recoil';

ReactDOM.render(
    <React.StrictMode>
        <RecoilRoot>
            <MsalComponent />
        </RecoilRoot>
    </React.StrictMode>,
    document.getElementById("root")
);