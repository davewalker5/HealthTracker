import sys
import sqlite3

# When the dump file is re-loaded into an empty database, these tables will already be
# populated by the migrations that created the empty database. To avoid UNIQUE key violations
# on load these tables are excluded from the dump
EXCLUSIONS = [
    "__EFMigrationsHistory",
    "BLOOD_PRESSURE_BAND",
    "BMI_BAND",
    "SPO2_BAND"
]

def dump_table_data(db_path, output_file):
    print("")
    print(f"DATABASE FILE : {db_path}")
    print(f"DUMP FILE     : {output_file}")
    print("")

    # Connect to the database
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()

    # Open the output dump file
    with open(output_file, 'w') as f:
        # Get the list of user tables
        cursor.execute("SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%';")
        tables = [row[0] for row in cursor.fetchall()]
        
        # Iterate over them
        for table in tables:
            # Check this one isn't excluded
            if not table in EXCLUSIONS:
                # Get the column information
                cursor.execute(f"PRAGMA table_info({table})")
                column_names = [row[1] for row in cursor.fetchall()]
                columns = ", ".join(column_names)

                # Execute a select to retrieve the data
                cursor.execute(f"SELECT {columns} FROM {table};")
                rows = cursor.fetchall()
                
                # Iterate over the results
                for row in rows:
                    # Construct an INSERT statement to insert this row of data and write it to the dump file
                    values = ','.join(["'{}'".format(str(v).replace("'", "''")) if v is not None else 'NULL' for v in row])
                    f.write(f"INSERT INTO {table} ({columns}) VALUES({values});\n")

    conn.close()

dump_table_data(sys.argv[1], sys.argv[2])
