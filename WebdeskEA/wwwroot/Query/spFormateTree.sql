create proc spFormateTree
as
begin
	-- user -------
	delete from AspNetUsers
	where id not in ('bb8f8917-effb-4255-a6cc-b19e12ac3225')

	-- Tenenat ----
	truncate table tenants

	-- Tenant permission ---
	truncate table tenantPermissions

	-- Company ---
	delete from Companies

end

