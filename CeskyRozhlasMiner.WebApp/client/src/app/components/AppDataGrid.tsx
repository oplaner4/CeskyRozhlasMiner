import { DataGrid, DataGridProps, GridToolbar } from "@mui/x-data-grid";
import { gridPageSizes } from "app/utils/grid";
import { useState } from "react";


const AppDataGrid: React.FC<DataGridProps> = (props: DataGridProps) => {
    const [pageSize, setPageSize] = useState<number>(gridPageSizes[0]);

    return <DataGrid
        getRowId={(row): string => row.id}
        autoHeight
        pageSize={pageSize}
        onPageSizeChange={setPageSize}
        rowsPerPageOptions={gridPageSizes}
        
        components={{ Toolbar: GridToolbar }}
        componentsProps={{
            toolbar: {
                showQuickFilter: true,
                quickFilterProps: { debounceMs: 500 },
            },
        }}
        disableColumnSelector
        disableDensitySelector

        //checkboxSelection
        disableSelectionOnClick
        {...props}
    />;
}

export default AppDataGrid;


