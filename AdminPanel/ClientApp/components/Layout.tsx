import * as React from 'react';
import { NavMenu } from './NavMenu';

export class Layout extends React.Component<{}, {}> {
    public render() {
        return <div className='container'>
            <div className='nav-menu'>
                        <NavMenu />
                    </div>
                    <div className='row'>
                        <div className='col-md-12'>
                            {this.props.children}
                         </div>
                    </div>
                </div>;
    }
}
