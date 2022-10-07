import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import Menu from '@mui/material/Menu';
import MenuIcon from '@mui/icons-material/Menu';
import Avatar from '@mui/material/Avatar';
import Button from '@mui/material/Button';
import Tooltip from '@mui/material/Tooltip';
import MenuItem from '@mui/material/MenuItem';

import Logo from '../../static/msftLogo.png';
import { AppRoute, UseRoutes, iterateThroughRoutes, AppRouteGroup } from 'app/components/AppRoutes';
import { useNavigate } from 'react-router-dom';
import AppWrapper from './AppWrapper';
import { useRecoilValue } from 'recoil';
import { userAtom } from 'app/state/atom';
import { getInitials, str2Hsl } from 'app/utils/utilities';

const AppResponsiveBar = () => {
  const user = useRecoilValue(userAtom);

  // Component behaviour

  const [anchorElNav, setAnchorElNav] = React.useState<null | HTMLElement>(null);
  const [anchorElUser, setAnchorElUser] = React.useState<null | HTMLElement>(null);

  const navigate = useNavigate();

  const handleOpenNavMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorElNav(event.currentTarget);
  };
  const handleOpenUserMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorElUser(event.currentTarget);
  };

  const handleCloseNavMenu = () => {
    setAnchorElNav(null);
  };

  const handleCloseUserMenu = () => {
    setAnchorElUser(null);
  };

  return (
    <AppBar position="static" color="primary">
      <AppWrapper py={1}>
          <Toolbar disableGutters>
              <Typography
                variant="h6"
                noWrap
                component="a"
                onClick={() => navigate(UseRoutes[AppRoute.Default].path)}
                sx={{
                  mr: 2,
                  display: { xs: 'none', md: 'flex' },
                  fontFamily: 'monospace',
                  fontWeight: 700,
                  letterSpacing: '.3rem',
                  color: 'inherit',
                  textDecoration: 'none',
                }}
              >
                <img alt="logo" src={Logo} style={{ maxWidth: 100 }} />
              </Typography>
    
              <Box sx={{ flexGrow: 1, display: { xs: 'flex', md: 'none' } }}>
                <IconButton
                  size="large"
                  aria-label="account of current user"
                  aria-controls="menu-appbar"
                  aria-haspopup="true"
                  onClick={handleOpenNavMenu}
                  color="inherit"
                >
                  <MenuIcon />
                </IconButton>
                <Menu
                  id="menu-appbar"
                  anchorEl={anchorElNav}
                  anchorOrigin={{
                    vertical: 'bottom',
                    horizontal: 'left',
                  }}
                  keepMounted
                  transformOrigin={{
                    vertical: 'top',
                    horizontal: 'left',
                  }}
                  open={Boolean(anchorElNav)}
                  onClose={handleCloseNavMenu}
                  sx={{
                    display: { xs: 'block', md: 'none' },
                  }}
                >
                  {iterateThroughRoutes().filter(r => UseRoutes[r].group === AppRouteGroup.Default
                      && UseRoutes[r].inMenu
                      && (user !== null || !UseRoutes[r].beAuthorized)
                  ).map(route =>
                      <MenuItem key={route} onClick={() => { handleCloseNavMenu(); navigate(UseRoutes[route].path); }}>
                        <Typography textAlign="center">
                          {UseRoutes[route].menuTitle}
                        </Typography>
                      </MenuItem>
                  )}
                </Menu>
              </Box>
    
              <Typography
                variant="h5"
                noWrap
                component="a"
                href=""
                sx={{
                  mr: 2,
                  display: { xs: 'flex', md: 'none' },
                  flexGrow: 1,
                  fontFamily: 'monospace',
                  fontWeight: 700,
                  letterSpacing: '.3rem',
                  color: 'inherit',
                  textDecoration: 'none',
                }}
              >
                <img alt="logo" src={Logo} style={{ maxWidth: 100 }} />
              </Typography>
              <Box sx={{ flexGrow: 1, display: { xs: 'none', md: 'flex' } }}>
                {iterateThroughRoutes().filter(r => UseRoutes[r].group === AppRouteGroup.Default
                      && UseRoutes[r].inMenu
                      && (user !== null || !UseRoutes[r].beAuthorized)
                  ).map(route =>
                    <Button
                      key={route}
                      onClick={()=> { handleCloseNavMenu(); navigate(UseRoutes[route].path); }}
                      sx={{ my: 2, color: 'white', display: 'block' }}
                    >
                        {UseRoutes[route].menuTitle}
                    </Button>
                )}
              </Box>
    
              <Box sx={{ flexGrow: 0 }}>
                <Tooltip title="Open settings">
                  <IconButton onClick={handleOpenUserMenu} sx={{ p: 0 }}>
                    {user === null || user.displayName === undefined ?
                      <Avatar alt="Not signed user" />
                      : 
                      <Avatar sx={{ bgcolor: str2Hsl(user.displayName) }} alt={user.displayName}>
                        {getInitials(user.displayName, 2).join(' ')}
                      </Avatar>
                    }
                  </IconButton>
                </Tooltip>
                <Menu
                  sx={{ mt: '45px' }}
                  id="menu-appbar"
                  anchorEl={anchorElUser}
                  anchorOrigin={{
                    vertical: 'top',
                    horizontal: 'right',
                  }}
                  keepMounted
                  transformOrigin={{
                    vertical: 'top',
                    horizontal: 'right',
                  }}
                  open={Boolean(anchorElUser)}
                  onClose={handleCloseUserMenu}
                >
                    {iterateThroughRoutes().filter(r => UseRoutes[r].group === AppRouteGroup.User
                        && UseRoutes[r].inMenu
                        && ((user === null && !UseRoutes[r].beAuthorized) || (user !== null && UseRoutes[r].beAuthorized))
                    ).map(route =>
                      <MenuItem key={route} onClick={()=> { handleCloseUserMenu(); navigate(UseRoutes[route].path); }}>
                          <Typography textAlign="center">
                            {UseRoutes[route].menuTitle}
                          </Typography>
                      </MenuItem>
                    )}
                </Menu>
              </Box>
          </Toolbar>
      </AppWrapper>
    </AppBar>
  );
};
export default AppResponsiveBar;