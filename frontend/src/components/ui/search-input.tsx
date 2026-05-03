import { Search } from "lucide-react";
import { InputGroup, InputGroupAddon, InputGroupInput } from "./input-group";
import type { ComponentProps } from "react";

export function SearchInput({
  placeholder,
  ...props
}: { placeholder?: string } & ComponentProps<typeof InputGroup>) {
  return (
    <InputGroup {...props}>
      <InputGroupAddon>
        <Search />
      </InputGroupAddon>
      <InputGroupInput type="search" placeholder={placeholder} />
    </InputGroup>
  );
}
